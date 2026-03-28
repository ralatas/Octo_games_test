using System;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

namespace Scriprs.Service.SaveLoad
{
    public interface IStringProtector
    {
        string Protect(string plainText);
        string Unprotect(string protectedText);
    }

    public interface ISaveLoadService
    {
        bool Exists(string key);
        void Save<T>(string key, T data);
        T Load<T>(string key, T fallback = default);
        bool TryLoad<T>(string key, out T data);
        void Delete(string key);
    }

    public sealed class JsonFileSaveLoadService : ISaveLoadService
    {
        private readonly string _rootPath;
        private readonly IStringProtector _protector;

        public JsonFileSaveLoadService(string rootPath = null, IStringProtector protector = null)
        {
            _rootPath = string.IsNullOrWhiteSpace(rootPath)
                ? Application.persistentDataPath
                : rootPath;

            _protector = protector;
        }

        public bool Exists(string key)
        {
            return File.Exists(BuildPath(key));
        }

        public void Save<T>(string key, T data)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentException("Save key cannot be null or empty.", nameof(key));

            string path = BuildPath(key);
            string directory = Path.GetDirectoryName(path);

            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            string json = JsonConvert.SerializeObject(data, Formatting.Indented);

            if (_protector != null)
                json = _protector.Protect(json);

            File.WriteAllText(path, json);
        }

        public T Load<T>(string key, T fallback = default)
        {
            if (TryLoad<T>(key, out var data))
                return data;

            return fallback;
        }

        public bool TryLoad<T>(string key, out T data)
        {
            data = default;

            if (string.IsNullOrWhiteSpace(key))
                return false;

            string path = BuildPath(key);

            if (!File.Exists(path))
                return false;

            try
            {
                string content = File.ReadAllText(path);

                if (string.IsNullOrWhiteSpace(content))
                    return false;

                if (_protector != null)
                    content = _protector.Unprotect(content);

                data = JsonConvert.DeserializeObject<T>(content);

                return data != null;
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"Failed to load save file '{key}': {ex.Message}");
                return false;
            }
        }

        public void Delete(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                return;

            string path = BuildPath(key);

            if (File.Exists(path))
                File.Delete(path);
        }

        private string BuildPath(string key)
        {
            return Path.Combine(_rootPath, key + ".json");
        }
    }
}
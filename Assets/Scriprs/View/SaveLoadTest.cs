using System;
using Scriprs.Service.SaveLoad;
using UnityEngine;

namespace Scriprs.View
{
    [Serializable]
    public class PlayerProgress
    {
        public int Level;
        public int Coins;
    }

    [Serializable]
    public class GameSettings
    {
        public bool MusicEnabled;
        public float Volume;
    }

    public class SaveLoadTest: MonoBehaviour
    {
        private ISaveLoadService saveLoad;

        private IStringProtector protector;
        private ISaveLoadService saveLoadEncripted;

        private void Awake()
        {
            saveLoad = new JsonFileSaveLoadService();

            protector = new AesStringProtector("SuperSecretKey", "InitVector");
            saveLoadEncripted = new JsonFileSaveLoadService(protector: protector);
        }

        private void Start()
        {
            saveLoad.Save("player_progress", new PlayerProgress
            {
                Level = 5,
                Coins = 1200
            });

            saveLoadEncripted.Save("settings", new GameSettings
            {
                MusicEnabled = true,
                Volume = 0.8f
            });

            GameSettings settings = saveLoadEncripted.Load("settings", new GameSettings());
            Debug.Log($"Volume: {settings.Volume} Music is play: {settings.MusicEnabled}");

            PlayerProgress progress = saveLoad.Load("player_progress", new PlayerProgress());
            Debug.Log($"Level: {progress.Level} Coins: {progress.Coins}");
        }
    }
}
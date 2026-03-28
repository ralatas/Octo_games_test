using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Scriprs.Service.SaveLoad
{
    public sealed class AesStringProtector : IStringProtector
    {
        private readonly byte[] _key;
        private readonly byte[] _iv;

        public AesStringProtector(string secretKey, string ivSecret)
        {
            _key = CreateKey(secretKey);
            _iv = CreateIV(ivSecret);
        }

        public string Protect(string plainText)
        {
            using Aes aes = Aes.Create();
            aes.Key = _key;
            aes.IV = _iv;

            ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

            using MemoryStream ms = new MemoryStream();
            using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
            using (StreamWriter sw = new StreamWriter(cs))
            {
                sw.Write(plainText);
            }

            return Convert.ToBase64String(ms.ToArray());
        }

        public string Unprotect(string protectedText)
        {
            byte[] encryptedBytes = Convert.FromBase64String(protectedText);

            using Aes aes = Aes.Create();
            aes.Key = _key;
            aes.IV = _iv;

            ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

            using MemoryStream ms = new MemoryStream(encryptedBytes);
            using CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
            using StreamReader sr = new StreamReader(cs);

            return sr.ReadToEnd();
        }

        private static byte[] CreateKey(string key)
        {
            using SHA256 sha256 = SHA256.Create();
            return sha256.ComputeHash(Encoding.UTF8.GetBytes(key));
        }

        private static byte[] CreateIV(string ivSecret)
        {
            using MD5 md5 = MD5.Create();
            return md5.ComputeHash(Encoding.UTF8.GetBytes(ivSecret));
        }
    }
}
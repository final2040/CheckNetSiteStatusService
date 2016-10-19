using System;
using System.Security.Cryptography;
using System.Text;
using Data;

namespace Services
{
    public class Md5Key : ICryptoKey
    {
        private byte[] _key;
        public Md5Key(string key):this(key,10){}
        public Md5Key(int hashes):this("default",hashes){ }
        public Md5Key(string key, int hashes)
        {
            this.Key = key;
            this.Hashes = hashes;
        }

        public string Key
        {
            get { return Encoding.UTF8.GetString(_key); }
            set { _key = Encoding.UTF8.GetBytes(value); }
        }

        public int Hashes { get; set; }

        public byte[] GetKey()
        {
            return GetKey(_key);
        }

        public byte[] GetKey(string key)
        {
            byte[] keyBytes = Encoding.UTF8.GetBytes(key);
            return GetKey(keyBytes);
        }

        public byte[] GetKey(byte[] keyBytes)
        {
            var md5Hash = MD5.Create();
            var resultKey = keyBytes;
            for (int i = 0; i < Hashes; i++)
            {
                resultKey = md5Hash.ComputeHash(resultKey);
            }
            md5Hash.Dispose();
            return resultKey;
        }

        public string GetBase64Key()
        {
            return Convert.ToBase64String(GetKey(_key));
        }

        public string GetBase64Key(string key)
        {
            return Convert.ToBase64String(GetKey(key));
        }

        public string GetBase64Key(byte[] keyBytes)
        {
            return Convert.ToBase64String(GetKey());
        }

    }
}
using System;
using System.IO;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using Data;

namespace Services
{
    public class AesEncryptor : IEncryptor
    {
        private ICryptoKey _keyObject;

        public AesEncryptor(ICryptoKey keyObject)
        {
            _keyObject = keyObject;
        }

        public string Base64Encrypt(byte[] data)
        {
            return Convert.ToBase64String(Encrypt(data));
        }
        public string Base64Encrypt(string data)
        {
            return Convert.ToBase64String(Encrypt(data));
        }

        public byte[] Encrypt(string data)
        {
            return Encrypt(Encoding.UTF8.GetBytes(data));
        }
        public byte[] Encrypt(byte[] data)
        {

            byte[] encryptedData;
            byte[] result;
            if (data == null || data.Length <= 0)
            {
                throw new ArgumentException("Los datos no pueden estár vacios.");
            }
            using (Aes aesAlgorithm = Aes.Create())
            {
                aesAlgorithm.Key = _keyObject.GetKey();
                aesAlgorithm.GenerateIV();

                ICryptoTransform encryptor = aesAlgorithm.CreateEncryptor(aesAlgorithm.Key, aesAlgorithm.IV);
                encryptedData = encryptor.TransformFinalBlock(data, 0, data.Length);
                result = new byte[aesAlgorithm.IV.Length + encryptedData.Length];
                aesAlgorithm.IV.CopyTo(result, 0);
                encryptedData.CopyTo(result, aesAlgorithm.IV.Length);
            }
            return result;
        }

        public byte[] Decrypt(string encryptedBase64Data)
        {
            return Decrypt(Convert.FromBase64String(encryptedBase64Data));
        }
        public byte[] Decrypt(byte[] encryptedData)
        {
            byte[] result;
            byte[] iv;
            byte[] buffer;
            byte[] encrypted;
            if (encryptedData == null || encryptedData.Length <= 0)
            {
                throw new ArgumentException("Los datos no pueden estár vacios.");
            }
            using (Aes aesAlgorithm = Aes.Create())
            {
                iv = new byte[aesAlgorithm.IV.Length];
                encrypted = new byte[encryptedData.Length - aesAlgorithm.IV.Length];

                Array.Copy(encryptedData, iv, iv.Length);
                Array.Copy(encryptedData, iv.Length, encrypted, 0, encrypted.Length);

                aesAlgorithm.Key = _keyObject.GetKey();
                aesAlgorithm.IV = iv;

                ICryptoTransform encryptor = aesAlgorithm.CreateDecryptor(aesAlgorithm.Key, aesAlgorithm.IV);
                result = encryptor.TransformFinalBlock(encrypted, 0, encrypted.Length);

            }
            return result;
        }

        public string DecrypttoText(string encryptedBase64Data)
        {
            return Encoding.UTF8.GetString(Decrypt(encryptedBase64Data));
        }
        public string DecrypttoText(byte[] encryptedData)
        {
            return Encoding.UTF8.GetString(Decrypt(encryptedData));
        }

        public SecureString DecryptToSecureString(string encryptedBase64Data)
        {
            return DecrypttoText(encryptedBase64Data).ConvertToSecureString();
        }
        public SecureString DecryptToSecureString(byte[] encryptedData)
        {
            return DecrypttoText(encryptedData).ConvertToSecureString();
        }
    }
}
using System;
using System.Security;
using System.Security.Cryptography;
using System.Text;

namespace Services.Encription
{
    /// <summary>
    /// Encripta una serie de datos utilizando el algoritmo AES 
    /// </summary>
    public class AesEncryptor : IEncryptor
    {
        private ICryptoKey _keyObject;

        public AesEncryptor(ICryptoKey keyObject)
        {
            _keyObject = keyObject;
        }
        /// <summary>
        /// Encripta un array de 8-bits y devuelve la información encriptada
        /// codificada con digitos Base64
        /// </summary>
        /// <param name="data">Cadena de texto en base 64</param>
        /// <returns>Información encriptada en base64</returns>
        public string EncryptToBase64(byte[] data)
        {
            return Convert.ToBase64String(Encrypt(data));
        }

        /// <summary>
        /// Encripta una cadena de texto y devuelve la información encriptada
        /// codificada con digitos Base64
        /// </summary>
        /// <param name="data">Cadena de texto en base 64</param>
        /// <returns>Información encriptada en base64</returns>
        public string EncryptToBase64(string data)
        {
            return Convert.ToBase64String(Encrypt(data));
        }

        /// <summary>
        /// Encripta una cadena de texto
        /// </summary>
        /// <param name="text">Texto a encriptar</param>
        /// <returns>Array de 8-bits que contiene la información encriptada</returns>
        public byte[] Encrypt(string text)
        {
            return Encrypt(Encoding.UTF8.GetBytes(text));
        }

        /// <summary>
        /// Encripta un array de bytes de 8-bits
        /// </summary>
        /// <param name="text">Texto a encriptar</param>
        /// <returns>Array de 8-bits que contiene la información encriptada</returns>
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


        /// <summary>
        /// Desencripta la cadena especificada, que codifica datos binarios como digitos base-64
        /// a un array de 8-bits
        /// </summary>
        /// <param name="encryptedBase64Data">Datos binarios codificados como datos base-64</param>
        /// <returns>Array de 8-bits que contiene la información desencriptada</returns>
        public byte[] Decrypt(string encryptedBase64Data)
        {
            return Decrypt(Convert.FromBase64String(encryptedBase64Data));
        }

        /// <summary>
        /// Desencripta un array de 8-bits, a un array de 8-bits
        /// </summary>
        /// <param name="encryptedData">Array de 8-bits que contiene la información encriptada</param>
        /// <returns>Array de 8-bits que contiene la información desencriptada</returns>
        public byte[] Decrypt(byte[] encryptedData)
        {
            byte[] result;
            byte[] iv;
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

        /// <summary>
        /// Desencripta la cadena especificada, que codifica datos binarios como digitos base-64
        /// a una cadena de texto.
        /// </summary>
        /// <param name="encryptedBase64Data">Datos binarios codificados como datos base-64 que contienen la
        /// la información encriptada</param>
        /// <returns>Cadena de texto con la información desencriptada</returns>
        public string DecryptToText(string encryptedBase64Data)
        {
            return Encoding.UTF8.GetString(Decrypt(encryptedBase64Data));
        }
        /// <summary>
        /// Desencripta un array de 8-bits, a una cadena de texto
        /// </summary>
        /// <param name="encryptedData">Array de 8-bits con la información encriptada</param>
        /// <returns>Cadena de texto con la información desencriptada</returns>
        public string DecryptToText(byte[] encryptedData)
        {
            return Encoding.UTF8.GetString(Decrypt(encryptedData));
        }
        /// <summary>
        /// Desencripta la cadena especificada, que codifica datos binarios como digitos base-64
        /// a una cadena de texto seguro (SecureString).
        /// </summary>
        /// <param name="encryptedBase64Data">Datos binarios codificados como datos base-64 que contienen la
        /// la información encriptada</param>
        /// <returns>SecureString que contiene los datos desencriptados</returns>
        public SecureString DecryptToSecureString(string encryptedBase64Data)
        {
            return DecryptToText(encryptedBase64Data).ConvertToSecureString();
        }

        /// <summary>
        /// Desencripta un array de 8-bits, a una cadena de texto Seguro SecureString
        /// </summary>
        /// <param name="encryptedData">Array de 8-bits con la información encriptada</param>
        /// <returns>SecureString con la información desencriptada</returns>
        public SecureString DecryptToSecureString(byte[] encryptedData)
        {
            return DecryptToText(encryptedData).ConvertToSecureString();
        }
    }
}
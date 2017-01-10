using System;
using System.Security.Cryptography;
using System.Text;

namespace Services.Encription
{
    /// <summary>
    /// Clase Auxiliar que genera una llave MD5 a partir de una cadena de texto
    /// </summary>
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

        /// <summary>
        /// Propiedad que obtiene y establece la llave que será convertida.
        /// </summary>
        public string Key
        {
            get { return Encoding.UTF8.GetString(_key); }
            set { _key = Encoding.UTF8.GetBytes(value); }
        }

        /// <summary>
        /// Propiedad que obtiene y establece el número de Hashes que serán aplicados a la llave
        /// por default 10
        /// </summary>
        public int Hashes { get; set; }

        /// <summary>
        /// Genera el Hash MD5
        /// </summary>
        /// <returns></returns>
        public byte[] GetKey()
        {
            return GetKey(_key);
        }

        /// <summary>
        /// Obtiene un array de 8-bits con el hash de la llave provista
        /// </summary>
        /// <param name="key">Cadena de texto que contiene la llave</param>
        /// <returns>Array de 8-bits que contiene el hash de la llave</returns>
        public byte[] GetKey(string key)
        {
            byte[] keyBytes = Encoding.UTF8.GetBytes(key);
            return GetKey(keyBytes);
        }

        /// <summary>
        /// Obtiene un array de 8-bits con el hash de la llave provista
        /// </summary>
        /// <param name="keyBytes">Array de 8-bits</param>
        /// <returns>Array de 8-bits que contiene el hash de la llave</returns>
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

        /// <summary>
        /// Obtiene el hash de la llave codificada en Datos Binarios codificados como datos base-64 
        /// </summary>
        /// <returns>Hash de la llave codificada en datos binarios base-64</returns>
        public string GetBase64Key()
        {
            return Convert.ToBase64String(GetKey(_key));
        }

        /// <summary>
        /// Obtiene el hash de la llave provista como parametro en Datos Binarios codificados como datos base-64
        /// </summary>
        /// <param name="key">Llave</param>
        /// <returns>Hash de la llave codificada en datos binarios base-64</returns>
        public string GetBase64Key(string key)
        {
            return Convert.ToBase64String(GetKey(key));
        }

        /// <summary>
        /// Obtiene el hash de los datos binarios proporcionados como parametro en Datos Binarios binarios codificados
        /// como datos base-64
        /// </summary>
        /// <param name="keyBytes">Array de 8-bits con la llave a codificar</param>
        /// <returns>Hash de la llave codificada en datos binarios base-64</returns>
        public string GetBase64Key(byte[] keyBytes)
        {
            return Convert.ToBase64String(GetKey());
        }

    }
}
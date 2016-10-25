using System;
using System.IO;
using System.Xml.Serialization;
using Data;

namespace Services
{
    public class XmlHelper
    {

        public Configuration GetConfig(string configPath)
        {
            if (!File.Exists(configPath))
                throw new InvalidOperationException($"No se encuentra el archivo de configuración en: {configPath}");

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(Configuration));
            FileStream fileStream = new FileStream(configPath, FileMode.Open, FileAccess.Read);
            var config = xmlSerializer.Deserialize(fileStream) as Configuration;

            if (config == null)
                throw new InvalidOperationException("No se pudo obtener la configuración de retorno objeto nulo");

            fileStream.Close();
            return config;
        }
    }
}
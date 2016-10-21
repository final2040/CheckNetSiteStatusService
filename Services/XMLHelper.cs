using System;
using System.IO;
using System.Xml.Serialization;
using Data;

namespace Services
{
    public class XMLHelper
    {
       public XMLHelper(){}

        public Configuration GetConfig(string configPath)
        {
            Configuration config;

            if (!File.Exists(configPath))
                throw new InvalidOperationException(string.Format("No se encuentra el archivo de configuración en: {0}",
                    configPath));

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(Configuration));
            FileStream fileStream = new FileStream(configPath, FileMode.Open, FileAccess.Read);
            config = xmlSerializer.Deserialize(fileStream) as Configuration;

            if (config == null)
                throw new InvalidOperationException("No se pudo obtener la configuración de retorno objeto nulo");

            return config;
        }
    }
}
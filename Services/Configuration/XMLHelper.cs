using System;
using System.IO;
using System.Xml.Serialization;

namespace Services.Configuration
{
    /// <summary>
    /// Clase auxiliar encargada de desserializar la información contenida en un archivo xml de configuración
    /// </summary>
    public class XmlHelper
    {
        /// <summary>
        /// Desserializa un archivo XML de configuración
        /// </summary>
        /// <param name="configPath">Ruta al archivo a deserializar</param>
        /// <returns>Objeto de configuración</returns>
        public Data.Configuration.Configuration GetConfig(string configPath)
        {
            if (!File.Exists(configPath))
                throw new InvalidOperationException($"No se encuentra el archivo de configuración en: {configPath}");

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(Data.Configuration.Configuration));
            FileStream fileStream = new FileStream(configPath, FileMode.Open, FileAccess.Read);
            var config = xmlSerializer.Deserialize(fileStream) as Data.Configuration.Configuration;

            if (config == null)
                throw new InvalidOperationException("No se pudo obtener la configuración de retorno objeto nulo");

            fileStream.Close();
            return config;
        }
    }
}
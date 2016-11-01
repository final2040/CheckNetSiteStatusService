using System;
using System.IO;
using System.Reflection;
using Data;

namespace Services
{
    public class ConfigManager
    {
        private static Configuration _configuration;

        private static string _xmlConfigPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
            "config.xml");

        private ConfigManager() { }

        public static Configuration Configuration
        {
            get
            {
                return GetConfiguration();
            }
        }

        public static Configuration GetConfiguration(string xmlConfigPath)
        {
            if (_configuration == null)
            {
                _configuration = Deserialize(xmlConfigPath);
            }
            return _configuration;
        }

        public static Configuration GetConfiguration()
        {
            if (_configuration == null)
            {
                _configuration = Deserialize(_xmlConfigPath);
            }
            return _configuration;
        }

        public static void SetXmlConfigPath(string path)
        {
            if (_configuration == null)
            {
                _xmlConfigPath = path;
            }
            else
            {
                throw new InvalidOperationException("El archivo de configuración " +
                                                "solo puede establecerse antes de instanciar la configuración " +
                                                "por primera vez.");
            }
        }

        private static Configuration Deserialize(string xmlConfigPath)
        {
            try
            {
                return new XmlHelper().GetConfig(xmlConfigPath);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Ocurrió un error al recuperar la configuración, " +
                                                    $"verifique el archivo de configuración:{ex.Message}",ex);
            }
        }
    }
}
using System;
using System.IO;
using System.Reflection;

namespace Services.Configuration
{
    /// <summary>
    /// Clase encargada de obtener y proveer la configuración del sistema, se utiliza el patrón de diseño singleton
    /// por lo que una vez cargada la configuración se utiliza una unica instancia del objeto en toda la aplicación
    /// </summary>
    public class ConfigManager
    {
        private static Data.Configuration.Configuration _configuration;

        private static string _xmlConfigPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
            "config.xml");

        private ConfigManager() { }

        /// <summary>
        /// Propiedad que obtiene la configuración de la aplicación
        /// </summary>
        public static Data.Configuration.Configuration Configuration
        {
            get
            {
                return GetConfiguration();
            }
        }

        /// <summary>
        /// Obtiene la configuración del sistema, si la configuración no ha sido cargada
        /// </summary>
        /// <param name="xmlConfigPath">Ruta del archivo XML que contiene la configuración</param>
        /// <returns>Objeto Configuration que representa la configuración almacenada el en archivo</returns>
        public static Data.Configuration.Configuration GetConfiguration(string xmlConfigPath)
        {
            if (_configuration == null)
            {
                _configuration = Deserialize(xmlConfigPath);
            }
            return _configuration;
        }

        /// <summary>
        /// Obtiene la configuración del sistema
        /// </summary>
        /// <returns>Objeto Configuration que representa la configuración almacenada el en archivo</returns>
        public static Data.Configuration.Configuration GetConfiguration()
        {
            if (_configuration == null)
            {
                _configuration = Deserialize(_xmlConfigPath);
            }
            return _configuration;
        }

        /// <summary>
        /// Establece la ruta donde se encuentra el archivo de configuración, esta función solo se puede utilizar una 
        /// sola vez antes de la primera carga del archivo de configuración. Una vez cargada la configuración producira
        /// una excepción si es utilizada nuevamente.
        /// </summary>
        /// <param name="path">Ruta donde se encuentra el archivo en formato XML con la configuración</param>
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

        /// <summary>
        /// Obtiene un archivo de configuración a partir de un archivo XML de configuración
        /// </summary>
        /// <param name="xmlConfigPath">Ruta del archivo XML de configuración</param>
        /// <returns>Objeto Configuration que representa la configuración almacenada el en archivo.</returns>
        private static Data.Configuration.Configuration Deserialize(string xmlConfigPath)
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
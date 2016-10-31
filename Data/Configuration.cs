using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace Data
{
    [Serializable]
    public class Configuration
    {
        [Required(ErrorMessage = "Debe proporcionar la configuración de las pruebas de conexión")]
        [ValidateObject(ErrorMessage = "Ocurrieron errores al validar {0}")]
        public TestConfig TestConfig { get; set; }

        [Required(ErrorMessage = "Debe proporcionar por lo menos una ip para probar")]
        [ValidateCollection(ErrorMessage = "Error validando lista de Ips en indice {1}")]
        [XmlArrayItem("TCP", typeof(TcpTestConfiguration))]
        [XmlArrayItem("IP", typeof(PingTestConfiguration))]
        public List<TestConfigurationBase> Tests { get; set; }

        [Required(ErrorMessage = "Debe de proporcionar una configuración de correo electrónico")]
        [ValidateObject(ErrorMessage = "Ocurrieron errores al validar {0}")]
        public MailConfiguration MailConfiguration { get; set; }

        
    }
    [Serializable]
    public class MailConfiguration
    {
        [Required(ErrorMessage = "Debe de proporcionar una dirección de correo electrónico origen")]
        public string SendFrom { get; set; }

        [XmlArrayItem("Email")]
        [Required(ErrorMessage = "Debe de proporcionar al menos un correo electrónico de destino")]
        public List<string> Recipients { get; set; }

        [Required(ErrorMessage = "Debe de proporcionar un asunto para el correo")]
        [MaxLength(255, ErrorMessage = "El asunto es demasiado largo, no debe de pasar de 255 carácteres")]
        public string Subject { get; set; }

        [Required(ErrorMessage = "Debe de proporcionar un cuerpo para el correo")]
        public string Body { get; set; }

        public bool IsHtml { get; set; }

        [Required(ErrorMessage = "Debe de proporcionar las credenciales de acceso para el correo")]
        [ValidateObject(ErrorMessage = "Ocurrieron errores al validar las credenciales de correo:")]
        public SmtpCredential SmtpCredentials { get; set; }

        [Required(ErrorMessage = "Debe de proporcionar las configuraciones del servidor smtp")]
        [ValidateObject(ErrorMessage = "Ocurrieron errores al validar las configuraciones del servidor smtp:")]
        public SmtpConfiguration SmtpConfiguration { get; set; }
    }
}
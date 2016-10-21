using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace Data
{
    [Serializable]
    public class Configuration
    {
        [Required(ErrorMessage = "Debe de proporcionar un tiempo de espera")]
        public int WaitTime { get; set; }

        [Required(ErrorMessage = "Debe de proporcionar un tiempo limite")]
        public int TimeOut { get; set; }

        [Required(ErrorMessage = "Debe proporcionar por lo menos una ip para probar")]
        [ValidateCollection(ErrorMessage = "Error validando lista de Ips en indice {1}")]
        public List<IP> IpToTest { get; set; }

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
    [Serializable]
    public class IP
    {
        [XmlAttribute]
        [Required(ErrorMessage = "Debe de proporcionar una dirección IP")]
        [RegularExpression("^(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$", 
            ErrorMessage = "El formato de la ip no es válido debe de coincidir con la mascara xxx.xxx.xxx.xxx")]
        public string Address { get; set; }

        [XmlAttribute]
        [Range(0, 65535, ErrorMessage = "El valor del puerto es inválido debe de ser entre 0 y 65535")]
        public int Port { get; set; }

        [XmlText]
        [Required(ErrorMessage = "Debe de proporcionar un Nombre para la conexion")]
        [MaxLength(80, ErrorMessage = "El nombre no puede exceder los 80 carácteres")]
        public string Name { get; set; }
    }
}
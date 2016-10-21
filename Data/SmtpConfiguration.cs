using System;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace Data
{
    [Serializable]
    public class SmtpConfiguration
    {
        public SmtpConfiguration(string host, int port, bool useSsl)
        {
            Host = host;
            Port = port;
            UseSsl = useSsl;
        }

        public SmtpConfiguration(){}
        [XmlAttribute]
        [Required(ErrorMessage = "Debe de proporcionar un servidor SMTP al cual conectarse")]
        [MaxLength(255, ErrorMessage = "El nombre del servidor SMTP es demasiado largo")]
        public string Host { get; set; }

        [XmlAttribute]
        [Required(ErrorMessage = "Debe de proporcionar un puerto SMTP al cual conectarse")]
        [Range(1, 65535, ErrorMessage = "El valor del puerto es inválido debe de ser entre 1 y 65535")]
        public int Port { get; set; }

        [XmlAttribute]
        [Required]
        public bool UseSsl { get; set; }
    }
}
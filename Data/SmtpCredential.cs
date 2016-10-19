using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices.ComTypes;
using System.Xml.Serialization;

namespace Data
{
    [Serializable]
    public class SmtpCredential
    {
        public SmtpCredential()
        {
        }

        public SmtpCredential(string password, string userName)
        {
            Password = password;
            UserName = userName;
        }
        [XmlAttribute]
        [Required(ErrorMessage = "Debe de proporcionar un nombre de usuario")]
        public string UserName { get; set; }
        [XmlAttribute]
        [Required(ErrorMessage = "Debe de proporcionar una contraseña")]
        public string Password { get; set; }
    }
}
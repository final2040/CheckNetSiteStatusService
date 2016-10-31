using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace Data
{
    public class TcpTestConfiguration : TestConfigurationBase
    {
        [Required(ErrorMessage = "Debe de proporcionar un número de puerto.")]
        [Range(1, 65535, ErrorMessage = "El número de puerto está fuera de intervalo.")]
        [XmlAttribute]
        public int Port { get; set; }

        [XmlAttribute]
        public int TimeOutMilliSeconds { get; set; }

        public override string ToString()
        {
            return $"Configuración TCPTest Nombre: {Name}, Host: {Host}, Puerto: {Port}, TimeOut: {TimeOutMilliSeconds}";
        }
    }
}
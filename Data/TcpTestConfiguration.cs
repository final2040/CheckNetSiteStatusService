using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace Data
{
    public class TcpTestConfiguration : TestConfigurationBase
    {
        [Required(ErrorMessage = "Debe de proporcionar un n�mero de puerto.")]
        [Range(1, 65535, ErrorMessage = "El n�mero de puerto est� fuera de intervalo.")]
        [XmlAttribute]
        public int Port { get; set; }

        [XmlAttribute]
        public int TimeOutMilliSeconds { get; set; }

        public override string ToString()
        {
            return $"Configuraci�n TCPTest Nombre: {Name}, Host: {Host}, Puerto: {Port}, TimeOut: {TimeOutMilliSeconds}";
        }
    }
}
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace Data
{
    public class TestConfigurationBase
    {
        [Required(ErrorMessage = "Debe de especificar un host a probar.")]
        [XmlAttribute]
        public string Host { get; set; }

        [Required(ErrorMessage = "Debe de especificar un nombre para este host.")]
        [StringLength(25, ErrorMessage = "El nombre del host no debe de exceder los 25 carácteres.")]
        [XmlText]
        public string Name { get; set; }
    }
}
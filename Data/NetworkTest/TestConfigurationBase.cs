using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace Data.NetworkTest
{
    /// <summary>
    /// Configuración base de la pruebas de red a realizar.
    /// </summary>
    public abstract class TestConfigurationBase
    {
        /// <summary>
        /// Propiedad que obtiene o establece el Host a probar
        /// </summary>
        [Required(ErrorMessage = "Debe de especificar un host a probar.")]
        [XmlAttribute]
        public string Host { get; set; }

        /// <summary>
        /// Propiedad que obtiene o establece el nombre de la conexión a probar
        /// </summary>
        [Required(ErrorMessage = "Debe de especificar un nombre para este host.")]
        [StringLength(255, ErrorMessage = "El nombre del host no debe de exceder los 25 carácteres.")]
        [XmlText]
        public string Name { get; set; }
    }
}
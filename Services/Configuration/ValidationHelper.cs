using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Data.Configuration;

namespace Services.Configuration
{
    /// <summary>
    /// Clase de auxiliar para la validación de objetos,
    /// se utiliza para validar la configuración de la aplicación,
    /// esta clase utiliza la Clase Validator del nombre de espacio DataAnnotations
    /// para realizar las validaciones.
    /// </summary>
    public class ValidationHelper
    {
        private readonly List<ValidationResult> _results = new List<ValidationResult>();

        /// <summary>
        /// Intenta validar un objeto
        /// </summary>
        /// <param name="obj">Objeto a validar</param>
        /// <returns>Resultados de la validación</returns>
        public ObjectValidationResults TryValidate(object obj)
        {
            var context = new ValidationContext(obj,null,null);
            var results = new List<ValidationResult>();
            var objectValidationResults = new ObjectValidationResults();
            objectValidationResults.IsValid = Validator.TryValidateObject(obj, context, results,true);

            foreach ( var result in results )
            {
                objectValidationResults.Add(result);
            }
            return objectValidationResults;
        }
    }

    /// <summary>
    /// Representa un conjunto de resultados de la validacion de una clase.
    /// </summary>
    public class ObjectValidationResults
    {
        private readonly List<ValidationResult> _results = new List<ValidationResult>();
        public bool IsValid { get; set; }
        public List<ValidationResult> Results { get { return _results; } }

        /// <summary>
        /// Añade un resultado a la lista de resultados
        /// </summary>
        /// <param name="result">Resultado de la validación</param>
        public void Add(ValidationResult result)
        {
            _results.Add(result);
        }

        /// <summary>
        /// Muestra una representación en cadena de texto del conjunto de validaciones.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            PrintResult(stringBuilder, _results, 0);
            return stringBuilder.ToString();
        }

        /// <summary>
        /// Da formato a los resultados de la validación
        /// </summary>
        /// <param name="stringBuilder"></param>
        /// <param name="validationResults"></param>
        /// <param name="indent"></param>
        private void PrintResult(StringBuilder stringBuilder, IEnumerable<ValidationResult> validationResults, int indent)
        {
            foreach (var result in validationResults)
            {
                stringBuilder.Append(SetIndent(indent));
                stringBuilder.AppendLine(result.ErrorMessage);
                if (result is CompositeValidationResults)
                {
                    PrintResult(stringBuilder,((CompositeValidationResults)result).Results,indent +1);
                }
            }
        }

        /// <summary>
        /// Agrega una sangria a cada resultado hijo
        /// </summary>
        /// <param name="indent">Número de indentación</param>
        /// <returns>Espacio de indentación</returns>
        private string SetIndent(int indent)
        {
            string indentSpace = "";
            char indentChar = '\t';
            for (int i = 0; i < indent; i++)
            {
                indentSpace += indentChar;
            }
            return indentSpace;
        }
    }
}

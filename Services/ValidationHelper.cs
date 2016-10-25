using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Data;

namespace Services
{
    public class ValidationHelper
    {
        private readonly List<ValidationResult> _results = new List<ValidationResult>();

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

    public class ObjectValidationResults
    {
        private readonly List<ValidationResult> _results = new List<ValidationResult>();
        public bool IsValid { get; set; }
        public List<ValidationResult> Results { get { return _results; } }

        public void Add(ValidationResult result)
        {
            _results.Add(result);
        }

        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            PrintResult(stringBuilder, _results, 0);
            return stringBuilder.ToString();
        }

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

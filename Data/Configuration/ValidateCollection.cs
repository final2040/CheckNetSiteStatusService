using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Data.Configuration
{
    public class ValidateCollectionAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var results = new List<ValidationResult>();
            
            
            if (value is IEnumerable)
            {
                var index = 0;
                foreach (var member in (IEnumerable)value)
                {
                    var memberResults = new List<ValidationResult>();
                    var context = new ValidationContext(member);
                    var memberIsValid = Validator.TryValidateObject(member, context, memberResults,true);
                    
                    if (!memberIsValid)
                    {
                        var memberCompositeResults = new CompositeValidationResults(string.Format(ErrorMessage, validationContext.DisplayName, index));
                        memberResults.ForEach(memberCompositeResults.AddResult);
                        results.Add(memberCompositeResults);
                    }
                    index++;
                }
                if ( results.Count != 0)
                {
                    var compositeResults = new CompositeValidationResults(string.Format("Errors Validating {0} collection", validationContext.DisplayName));
                    results.ForEach(compositeResults.AddResult);
                    return compositeResults;
                }
                return ValidationResult.Success;
            }
            return new CompositeValidationResults(string.Format("No se puede validar la propiedad {0} no es una lista o coleccion",validationContext.DisplayName));
        }
    }
}
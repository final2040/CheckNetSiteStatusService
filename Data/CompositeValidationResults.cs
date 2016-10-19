using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Data
{
    public class CompositeValidationResults:ValidationResult
    {
        private readonly List<ValidationResult> _results = new List<ValidationResult>();
        public CompositeValidationResults(string errorMessage):base(errorMessage){}

        public CompositeValidationResults(string errorMessage, IEnumerable<string> displayName):base (errorMessage,displayName){}

        protected CompositeValidationResults(ValidationResult validationResult):base(validationResult){}

        public IEnumerable<ValidationResult> Results { get { return _results; } }

        public void AddResult(ValidationResult result)
        {
            _results.Add(result);
        }
    }
}
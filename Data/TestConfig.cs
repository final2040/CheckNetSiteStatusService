using System.ComponentModel.DataAnnotations;

namespace Data
{
    public class TestConfig
    {
        [Required(ErrorMessage = "Debe de proporcionar un tiempo de espera")]
        public int WaitTimeSeconds { get; set; }

        [Required(ErrorMessage = "Debe de proporcionar un tiempo limite")]
        public int TimeOutSeconds { get; set; }
    }
}
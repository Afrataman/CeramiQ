using System.ComponentModel.DataAnnotations;

namespace CeramiQ.Web.ViewModels
{
    public class NaturalLanguageQueryViewModel
    {
        [Required(ErrorMessage = "Lütfen bir soru yazınız.")]
        [Display(Name = "CeramiQ'ya Sor")]
        public string Question { get; set; } = string.Empty;

        public string GeneratedSql { get; set; } = string.Empty;

        public string Explanation { get; set; } = string.Empty;

        
        
       
        public string ErrorMessage { get; set; } = string.Empty;
        public List<string> ResultColumns { get; set; } = new();

        public List<Dictionary<string, string>> ResultRows { get; set; } = new();
    }


}
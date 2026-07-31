using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace CeramiQ.Web.ViewModels
{
    public class ProductionOrderCreateViewModel
    {
        [Required(ErrorMessage = "Emir numarası zorunludur.")]
        [StringLength(30)]
        [Display(Name = "Emir Numarası")]
        public string OrderNumber { get; set; } = string.Empty;

        [Range(1, int.MaxValue,
            ErrorMessage = "Bir ürün seçmelisiniz.")]
        [Display(Name = "Ürün")]
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Üretim hattı zorunludur.")]
        [StringLength(50)]
        [Display(Name = "Üretim Hattı")]
        public string ProductionLine { get; set; } = string.Empty;

        [Range(
    0.01,
    double.MaxValue,
    ErrorMessage = "Planlanan miktar sıfırdan büyük olmalıdır.")]
        [Display(Name = "Planlanan Miktar")]
        public decimal PlannedQuantity { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Başlangıç Tarihi")]
        public DateTime StartDate { get; set; } = DateTime.Today;

        [DataType(DataType.Date)]
        [Display(Name = "Teslim Tarihi")]
        public DateTime DueDate { get; set; } =
            DateTime.Today.AddDays(7);

        public List<SelectListItem> Products { get; set; } = new();
    }
}
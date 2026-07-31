using System.ComponentModel.DataAnnotations;

namespace CeramiQ.Web.ViewModels
{
    public class ProductionOrderUpdateViewModel
    {
        public int Id { get; set; }

        public string OrderNumber { get; set; } = string.Empty;

        public string ProductName { get; set; } = string.Empty;

        public decimal PlannedQuantity { get; set; }

        [Range(
            0,
            double.MaxValue,
            ErrorMessage = "Üretilen miktar negatif olamaz.")]
        [Display(Name = "Üretilen Miktar")]
        public decimal ProducedQuantity { get; set; }

        [Range(
            0,
            double.MaxValue,
            ErrorMessage = "Fire miktarı negatif olamaz.")]
        [Display(Name = "Fire Miktarı")]
        public decimal ScrapQuantity { get; set; }

        [Required(ErrorMessage = "Durum seçmelisiniz.")]
        [Display(Name = "Üretim Durumu")]
        public string Status { get; set; } = "Planlandı";
    }
}
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace CeramiQ.Web.ViewModels
{
    public class StockMovementCreateViewModel
    {
        [Range(
            1,
            int.MaxValue,
            ErrorMessage = "Lütfen bir ürün seçiniz.")]
        [Display(Name = "Ürün")]
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Lütfen hareket türünü seçiniz.")]
        [Display(Name = "Hareket Türü")]
        public string MovementType { get; set; } = string.Empty;

        [Range(
     0.01,
     999999999,
     ErrorMessage = "Miktar sıfırdan büyük olmalıdır.")]
        [Display(Name = "Miktar")]
        public decimal Quantity { get; set; }

        [StringLength(
            250,
            ErrorMessage = "Açıklama en fazla 250 karakter olabilir.")]
        [Display(Name = "Açıklama")]
        public string Description { get; set; } = string.Empty;

        public List<SelectListItem> Products { get; set; } = new();
    }
}
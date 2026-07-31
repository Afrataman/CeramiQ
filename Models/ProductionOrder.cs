using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace CeramiQ.Web.Models
{
    public class ProductionOrder
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Üretim emri numarası zorunludur.")]
        [StringLength(30)]
        public string OrderNumber { get; set; } = string.Empty;

        [Range(1, int.MaxValue,
            ErrorMessage = "Bir ürün seçmelisiniz.")]
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Üretim hattı zorunludur.")]
        [StringLength(50)]
        public string ProductionLine { get; set; } = string.Empty;

        [Precision(18, 2)]
        [Range(0.01, double.MaxValue,
            ErrorMessage = "Planlanan miktar sıfırdan büyük olmalıdır.")]
        public decimal PlannedQuantity { get; set; }

        [Precision(18, 2)]
        [Range(0, double.MaxValue,
            ErrorMessage = "Üretilen miktar negatif olamaz.")]
        public decimal ProducedQuantity { get; set; }

        [Precision(18, 2)]
        [Range(0, double.MaxValue,
            ErrorMessage = "Fire miktarı negatif olamaz.")]
        public decimal ScrapQuantity { get; set; }

        [Required]
        public DateTime StartDate { get; set; } = DateTime.Today;

        [Required]
        public DateTime DueDate { get; set; } =
            DateTime.Today.AddDays(7);

        [Required]
        public string Status { get; set; } = "Planlandı";

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
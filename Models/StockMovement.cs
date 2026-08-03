using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace CeramiQ.Web.Models
{
    public class StockMovement
    {
        public int Id { get; set; }

        [Required]
        public int ProductId { get; set; }

        [Required]
        [StringLength(10)]
        public string MovementType { get; set; } = string.Empty;

        [Precision(18, 2)]
        public decimal Quantity { get; set; }

        [StringLength(250)]
        public string Description { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
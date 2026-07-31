using Microsoft.EntityFrameworkCore;
namespace CeramiQ.Web.Models
{
    public class Product
    {
        public int Id { get; set; }

        public string ProductCode { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public string Category { get; set; } = string.Empty;

        [Precision(18, 2)]
        public decimal StockQuantity { get; set; }

        [Precision(18, 2)]
        public decimal MinimumStockLevel { get; set; }

        public string Unit { get; set; } = "Adet";

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
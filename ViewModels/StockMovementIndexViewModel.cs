using Microsoft.AspNetCore.Mvc.Rendering;

namespace CeramiQ.Web.ViewModels
{
    public class StockMovementIndexViewModel
    {
        public int? ProductId { get; set; }

        public string MovementType { get; set; } = string.Empty;

        public List<SelectListItem> Products { get; set; } = new();

        public List<StockMovementListItemViewModel> Movements
        {
            get;
            set;
        } = new();
    }

    public class StockMovementListItemViewModel
    {
        public int Id { get; set; }

        public string ProductCode { get; set; } = string.Empty;

        public string ProductName { get; set; } = string.Empty;

        public string MovementType { get; set; } = string.Empty;

        public decimal Quantity { get; set; }

        public string Description { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }
    }
}
namespace CeramiQ.Web.ViewModels
{
    public class ProductionOrderListItemViewModel
    {
        public int Id { get; set; }

        public string OrderNumber { get; set; } = string.Empty;

        public string ProductCode { get; set; } = string.Empty;

        public string ProductName { get; set; } = string.Empty;

        public string ProductionLine { get; set; } = string.Empty;

        public decimal PlannedQuantity { get; set; }

        public decimal ProducedQuantity { get; set; }
        public decimal ScrapQuantity { get; set; }
        public DateTime DueDate { get; set; }

        public string Status { get; set; } = string.Empty;

        public bool IsDelayed =>
            DueDate.Date < DateTime.Today &&
            Status != "Tamamlandı";
    }
}
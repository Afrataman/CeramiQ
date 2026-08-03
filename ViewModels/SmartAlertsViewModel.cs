namespace CeramiQ.Web.ViewModels
{
    public class SmartAlertsViewModel
    {
        public List<CriticalStockAlertViewModel> CriticalStocks
        { get; set; } = new();

        public List<SmartProductionAlertViewModel> HighScrapOrders
        { get; set; } = new();

        public List<SmartProductionAlertViewModel> DelayedOrders
        { get; set; } = new();

        public int TotalAlertCount =>
            CriticalStocks.Count +
            HighScrapOrders.Count +
            DelayedOrders.Count;
    }

    public class CriticalStockAlertViewModel
    {
        public int ProductId { get; set; }

        public string ProductCode { get; set; } = string.Empty;

        public string ProductName { get; set; } = string.Empty;

        public decimal StockQuantity { get; set; }

        public decimal MinimumStockLevel { get; set; }

        public string Unit { get; set; } = string.Empty;
    }

    public class SmartProductionAlertViewModel
    {
        public int OrderId { get; set; }

        public string OrderNumber { get; set; } = string.Empty;

        public string ProductCode { get; set; } = string.Empty;

        public string ProductName { get; set; } = string.Empty;

        public string ProductionLine { get; set; } = string.Empty;

        public decimal ProducedQuantity { get; set; }

        public decimal ScrapQuantity { get; set; }

        public decimal ScrapRate { get; set; }

        public DateTime DueDate { get; set; }

        public string Status { get; set; } = string.Empty;

        public int DelayDayCount =>
            DueDate.Date < DateTime.Today
                ? (DateTime.Today - DueDate.Date).Days
                : 0;
    }
}
namespace CeramiQ.Web.Models
{
    public class DashboardViewModel
    {
        public int ProductCount { get; set; }

        public decimal TotalStock { get; set; }

        public int CriticalStockCount { get; set; }

        public int ActiveProductionOrderCount { get; set; }

        public int HighScrapOrderCount { get; set; }

        public int DelayedProductionOrderCount { get; set; }

        public int TotalAlertCount =>
            CriticalStockCount +
            HighScrapOrderCount +
            DelayedProductionOrderCount;
    }
}
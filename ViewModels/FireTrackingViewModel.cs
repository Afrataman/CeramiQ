using Microsoft.AspNetCore.Mvc.Rendering;

namespace CeramiQ.Web.ViewModels
{
    public class FireTrackingViewModel
    {
        public int? ProductId { get; set; }

        public string ProductionLine { get; set; } = string.Empty;

        public decimal TotalProducedQuantity { get; set; }

        public decimal TotalScrapQuantity { get; set; }

        public decimal OverallScrapRate { get; set; }

        public int HighRiskOrderCount { get; set; }

        public List<SelectListItem> Products { get; set; } = new();

        public List<SelectListItem> ProductionLines { get; set; } = new();

        public List<FireTrackingListItemViewModel> Orders { get; set; }
            = new();
    }

    public class FireTrackingListItemViewModel
    {
        public int Id { get; set; }

        public string OrderNumber { get; set; } = string.Empty;

        public string ProductCode { get; set; } = string.Empty;

        public string ProductName { get; set; } = string.Empty;

        public string ProductionLine { get; set; } = string.Empty;

        public decimal ProducedQuantity { get; set; }

        public decimal ScrapQuantity { get; set; }

        public decimal TotalProcessedQuantity { get; set; }

        public decimal ScrapRate { get; set; }

        public string Status { get; set; } = string.Empty;

        public DateTime DueDate { get; set; }

        public string RiskLevel
        {
            get
            {
                if (ScrapRate >= 10)
                {
                    return "Yüksek Risk";
                }

                if (ScrapRate >= 5)
                {
                    return "Dikkat";
                }

                return "Normal";
            }
        }
    }
}
namespace CeramiQ.Web.Models
{
    public class AlertExplanationViewModel
    {
        public string AlertType { get; set; } = string.Empty;

        public string Title { get; set; } = string.Empty;

        public int AffectedRecordCount { get; set; }

        public string Explanation { get; set; } = string.Empty;

        public string PossibleCause { get; set; } = string.Empty;

        public string RecommendedAction { get; set; } = string.Empty;
    }
}
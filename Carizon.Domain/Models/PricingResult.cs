namespace Carizon.Domain.Models
{
    public class PricingResult : BaseEntity
    {
        public Guid InspectionId { get; set; }
        public decimal BasePrice { get; set; }
        public decimal AdjustPrice { get; set; }
        public decimal MinPrice { get; set; }
        public decimal MaxPrice { get; set; }
        public decimal ConfidenceScore { get; set; }
        public DateTime CalculatedAt { get; set; }

        //Navigaton Properties
        public Inspection Inspection { get; set; }
    }
}

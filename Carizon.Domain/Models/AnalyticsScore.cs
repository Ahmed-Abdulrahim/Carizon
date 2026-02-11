namespace Carizon.Domain.Models
{
    public class AnalyticsScore : BaseEntity
    {
        public Guid InspectionId { get; set; }
        public decimal EngagmentScore { get; set; }
        public decimal BuyerIntentScore { get; set; }
        public int ViewCount { get; set; }
        public int SaveCount { get; set; }
        public int ContactCount { get; set; }
        public DateTime CalculatedAt { get; set; }

        //Nvigation Prop
        public Inspection Inspection { get; set; }
    }
}

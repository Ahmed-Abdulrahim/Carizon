namespace Carizon.Domain.Models
{
    public enum FinalMadeDecision
    {
        Publish,
        Hold,
        Reject,
    }
    public class FinalDecision : BaseEntity
    {
        public Guid InspectionId { get; set; }
        public decimal InspectionScore { get; set; }
        public decimal PricingConfidence { get; set; }
        public decimal EngagementScore { get; set; }
        public decimal FinalScore { get; set; }
        public FinalMadeDecision Decision { get; set; }
        public string? Reason { get; set; }
        public Guid DecidedBy { get; set; }
        public DateTime DecidedAt { get; set; }

        //Naviagtion Prop
        public Inspection Rule { get; set; }
        public ApplicationUser User { get; set; }
    }
}

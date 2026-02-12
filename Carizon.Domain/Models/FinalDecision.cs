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
        public Inspection Inspection { get; set; }
        public ApplicationUser User { get; set; }


        //Function
        public void MakeDecision(Guid userId, Func<Guid, bool> hasPermission, Guid sellerId)
        {
            if (userId == sellerId)
                throw new InvalidOperationException("Conflict of interest: user cannot decide on own inspection.");
            if (!hasPermission(userId))
                throw new UnauthorizedAccessException("User does not have permission to decide.");

            DecidedBy = userId;
            DecidedAt = DateTime.UtcNow;
        }

    }
}

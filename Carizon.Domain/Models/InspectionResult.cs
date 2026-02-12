namespace Carizon.Domain.Models
{
    public enum InspectionResultDecision
    {
        Accept,
        Reject,
    }
    public class InspectionResult : BaseEntity
    {

        public Guid InspectionId { get; set; }
        public Guid RuleId { get; set; }
        public int Score { get; set; }
        public string? Notes { get; set; }
        public InspectionResultDecision Decision { get; set; }
        public DateTime CreatedAt { get; set; }
        public decimal TotalScore => Score * Rule.Weight;


        //Navigation Properties
        public Inspection Inspection { get; set; }
        public InspectionRule Rule { get; set; }
    }
}

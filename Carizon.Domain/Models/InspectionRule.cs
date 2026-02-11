namespace Carizon.Domain.Models
{
    public class InspectionRule : BaseEntity
    {
        public string RuleName { get; set; }
        public string Category { get; set; }
        public decimal Weight { get; set; }
        public int MinAcceptableScore { get; set; }
        public bool IsActive { get; set; } = true;

        //Navigation Prop
        public ICollection<InspectionResult> InspectionResults { get; set; }

    }
}

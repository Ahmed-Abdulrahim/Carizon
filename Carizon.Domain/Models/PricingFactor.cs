namespace Carizon.Domain.Models
{
    public enum FactorType
    {
        Market,
        Condition,
        Age,
        Mileage
    }
    public class PricingFactor : BaseEntity
    {
        public string FactorName { get; set; }
        public FactorType FactorType { get; set; }
        public decimal Weight { get; set; }
        public bool IsActive { get; set; } = true;

        //Navigation Prop
        public ICollection<PricingResult> PricingResults { get; set; }
    }
}

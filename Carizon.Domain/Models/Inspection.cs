namespace Carizon.Domain.Models
{
    public enum InspectionStatus
    {
        pending,
        completed,
    }
    public class Inspection : BaseEntity
    {
        public string CarVin { get; set; }
        public string CarMake { get; set; }
        public string CarModel { get; set; }
        public int CarYear { get; set; }
        public int Mileage { get; set; }
        public Guid InspectorId { get; set; }
        public Guid SellerId { get; set; }
        public InspectionStatus Status { get; set; } = InspectionStatus.pending;
        public DateTime CreatedAt { get; set; }
        public DateTime? CompletedAt { get; set; }

        //Navigiation Properties
        public ApplicationUser Inspector { get; set; }
        public ApplicationUser Seller { get; set; }
        public ICollection<InspectionResult> InspectionResult { get; set; }
    }
}

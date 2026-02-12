namespace Carizon.Domain.Models
{
    public enum InspectionStatus
    {
        Pending,
        Completed,
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
        public InspectionStatus Status { get; set; } = InspectionStatus.Pending;
        public DateTime CreatedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public decimal FinalScore => InspectionResult.Sum(r => r.TotalScore);


        //Navigiation Properties
        public ApplicationUser Inspector { get; set; }
        public ApplicationUser Seller { get; set; }
        public ICollection<InspectionResult> InspectionResult { get; set; }
        public PricingResult PricingResult { get; set; }
        public AnalyticsScore AnalyticsScore { get; set; }
        public FinalDecision FinalDecision { get; set; }
        public ICollection<UserEvent> UserEvents { get; set; }

        //Function
        public void Complete()
        {
            if (Status == InspectionStatus.Completed)
                throw new InvalidOperationException("Inspection already completed");

            Status = InspectionStatus.Completed;
            CompletedAt = DateTime.UtcNow;
        }

        public void AssignInspector(Guid inspectorId)
        {
            if (Status != InspectionStatus.Pending)
                throw new InvalidOperationException("Cannot reassign completed inspection");

            InspectorId = inspectorId;
        }

        //Factory Method
        public static Inspection Create(string vin, string make, string model,
       int year, int mileage, Guid sellerId, Guid inspectorId)
        {
            if (string.IsNullOrWhiteSpace(vin) || vin.Length != 17)
                throw new ArgumentException("Invalid VIN format");

            if (year < 1900 || year > DateTime.UtcNow.Year + 1)
                throw new ArgumentException("Invalid car year");

            return new Inspection
            {
                CarVin = vin,
                CreatedAt = DateTime.UtcNow
            };
        }
    }
}

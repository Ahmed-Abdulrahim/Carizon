namespace Carizon.Domain.Models
{
    public enum EventType
    {
        View,
        Save,
        Contact,
    }
    public class UserEvent : BaseEntity
    {
        public Guid UserId { get; set; }
        public Guid InspectionId { get; set; }
        public EventType EventType { get; set; }
        public string? EventData { get; set; }
        public DateTime CreatedAt { get; set; }

        //Navigation Prop
        public ApplicationUser User { get; set; }
        public InspectionRule Rule { get; set; }
    }
}

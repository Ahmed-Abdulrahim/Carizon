namespace Carizon.Domain.Models
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName => $"{FirstName} {LastName}";

        //Navigatio Property
        public ICollection<RefreshToken> RefreshTokens { get; set; } = new HashSet<RefreshToken>();
        public ICollection<UserEvent> UserEvents { get; set; } = new List<UserEvent>();
        public ICollection<Inspection> Inspectors { get; set; } = new List<Inspection>();
        public ICollection<Inspection> Sellers { get; set; } = new List<Inspection>();
        public ICollection<FinalDecision> FinalDecisions { get; set; } = new List<FinalDecision>();

    }
}

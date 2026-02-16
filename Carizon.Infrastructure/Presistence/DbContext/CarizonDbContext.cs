namespace Carizon.Infrastructure.Presistence.DbContext
{
    public class CarizonDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
    {
        public CarizonDbContext(DbContextOptions<CarizonDbContext> options) : base(options) { }

        //DataBase
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<ApplicationRole> ApplicationRoles { get; set; }
        public DbSet<AnalyticsScore> AnalyticsScores { get; set; }
        public DbSet<FinalDecision> FinalDecisions { get; set; }
        public DbSet<Inspection> Inspections { get; set; }
        public DbSet<InspectionResult> InspectionResults { get; set; }
        public DbSet<InspectionRule> InspectionRules { get; set; }
        public DbSet<PricingFactor> PricingFactors { get; set; }
        public DbSet<PricingResult> PricingResults { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<UserEvent> UserEvents { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfigurationsFromAssembly(typeof(CarizonDbContext).Assembly);
        }
    }
}

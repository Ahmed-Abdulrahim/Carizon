
namespace Carizon.Infrastructure.Presistence.Configrations
{
    public class FinalDecisionConfigration : IEntityTypeConfiguration<FinalDecision>
    {
        public void Configure(EntityTypeBuilder<FinalDecision> builder)
        {
            builder.ToTable("FinalDecisions");
            builder.Property(f => f.InspectionScore).HasColumnType("decimal(5,2)").IsRequired();
            builder.Property(f => f.PricingConfidence).HasColumnType("decimal(5,2)").IsRequired();
            builder.Property(f => f.EngagementScore).HasColumnType("decimal(5,2)").IsRequired();
            builder.Property(f => f.FinalScore).HasColumnType("decimal(5,2)").IsRequired();
            builder.Property(f => f.Decision).HasConversion<string>().HasMaxLength(20).IsRequired();
            builder.Property(f => f.Reason).HasMaxLength(500).IsRequired(false);
            builder.Property(f => f.DecidedAt).HasDefaultValueSql("GETUTCDATE()");

            //RelationShip
            builder.HasOne(f => f.Inspection).WithOne(i => i.FinalDecision).HasForeignKey<FinalDecision>(f => f.InspectionId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(f => f.User).WithMany(a => a.FinalDecisions).HasForeignKey(f => f.DecidedBy)
                .OnDelete(DeleteBehavior.NoAction);

            //Index
            builder.HasIndex(f => f.Decision).HasDatabaseName("IX_FinalDecision_Decision");
            builder.HasIndex(f => f.DecidedAt).HasDatabaseName("IX_FinalDecision_DecideAt");

        }
    }
}

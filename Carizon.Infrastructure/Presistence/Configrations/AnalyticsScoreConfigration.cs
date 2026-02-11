
namespace Carizon.Infrastructure.Presistence.Configrations
{
    public class AnalyticsScoreConfigration : IEntityTypeConfiguration<AnalyticsScore>
    {
        public void Configure(EntityTypeBuilder<AnalyticsScore> builder)
        {
            builder.ToTable("AnalyticsScores", c =>
            {
                c.HasCheckConstraint("CK_AnalyticsScore_EngagementScore_Range",
                    "[EngagmentScore] >= 0 AND [EngagmentScore] <= 100");
                c.HasCheckConstraint("CK_AnalyticsScore_BuyerIntentScore_Range",
                    "[BuyerIntentScore] >= 0 AND [BuyerIntentScore] <= 100");
            });
            builder.Property(a => a.EngagmentScore).HasColumnType("decimal(5,2)").IsRequired();
            builder.Property(a => a.BuyerIntentScore).HasColumnType("decimal(5,2)").IsRequired();
            builder.Property(a => a.CalculatedAt).HasDefaultValueSql("GETUTCDATE()");

            //RelationShip
            builder.HasOne(a => a.Inspection).WithOne(i => i.AnalyticsScore).HasForeignKey<AnalyticsScore>(a => a.InspectionId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

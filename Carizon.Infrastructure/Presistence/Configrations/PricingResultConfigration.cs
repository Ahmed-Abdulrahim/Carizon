
namespace Carizon.Infrastructure.Presistence.Configrations
{
    public class PricingResultConfigration : IEntityTypeConfiguration<PricingResult>
    {
        public void Configure(EntityTypeBuilder<PricingResult> builder)
        {
            builder.ToTable("PricingResults", p =>
            {
                p.HasCheckConstraint("CK_PricingResults_ConfidenceScore_Range",
                    "[ConfidenceScore] >= 0 AND [ConfidenceScore] <= 100");
            });
            builder.Property(p => p.BasePrice).HasColumnType("decimal(18,2)").IsRequired();
            builder.Property(p => p.AdjustPrice).HasColumnType("decimal(18,2)").IsRequired();
            builder.Property(p => p.MinPrice).HasColumnType("decimal(18,2)").IsRequired();
            builder.Property(p => p.MaxPrice).HasColumnType("decimal(18,2)").IsRequired();
            builder.Property(p => p.ConfidenceScore).HasColumnType("decimal(5,2)").IsRequired();
            builder.Property(p => p.CalculatedAt).HasDefaultValueSql("GETUTCDATE()");

            //RelationShip
            builder.HasOne(p => p.Inspection)
                .WithOne(i => i.PricingResult)
                .HasForeignKey<PricingResult>(p => p.InspectionId)
                .OnDelete(DeleteBehavior.Cascade);

        }
    }
}

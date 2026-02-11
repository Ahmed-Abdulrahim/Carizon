
namespace Carizon.Infrastructure.Presistence.Configrations
{
    public class PricingFactorConfigration : IEntityTypeConfiguration<PricingFactor>
    {
        public void Configure(EntityTypeBuilder<PricingFactor> builder)
        {
            builder.ToTable("PricingFactors");
            builder.Property(p => p.FactorName).HasMaxLength(100).IsRequired();
            builder.Property(p => p.FactorType).HasConversion<string>().HasMaxLength(50).IsRequired();
            builder.Property(p => p.Weight).HasColumnType("decimal(5,2)").IsRequired();
        }
    }
}

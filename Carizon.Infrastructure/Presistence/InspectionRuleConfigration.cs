namespace Carizon.Infrastructure.Presistence
{
    public class InspectionRuleConfigration : IEntityTypeConfiguration<InspectionRule>
    {
        public void Configure(EntityTypeBuilder<InspectionRule> builder)
        {
            builder.ToTable("InspectionRules");
            builder.Property(i => i.RuleName).HasMaxLength(100).IsRequired();
            builder.Property(i => i.Category).HasMaxLength(50).IsRequired();
            builder.Property(i => i.Weight).HasColumnType("decimal(5,2)").IsRequired();
            builder.Property(i => i.MinAcceptableScore).IsRequired();
            builder.Property(i => i.IsActive).HasDefaultValue(true);

            //Index
            builder.HasIndex(i => i.Category);
        }
    }
}

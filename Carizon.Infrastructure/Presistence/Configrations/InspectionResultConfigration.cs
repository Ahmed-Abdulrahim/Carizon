
namespace Carizon.Infrastructure.Presistence.Configrations
{
    public class InspectionResultConfigration : IEntityTypeConfiguration<InspectionResult>
    {
        public void Configure(EntityTypeBuilder<InspectionResult> builder)
        {
            builder.ToTable("InspectionResults", t =>
            {
                t.HasCheckConstraint("CK_InspectionResults_Score_Range",
                    "[Score] >= 0 AND [Score] <= 100");
            });
            builder.Property(i => i.Score).IsRequired();
            builder.Property(i => i.Notes).HasMaxLength(500).IsRequired(false);
            builder.Property(i => i.Decision).HasConversion<string>().HasMaxLength(20).IsRequired();
            builder.Property(i => i.TotalScore).HasColumnType("decimal(5,2)").IsRequired();
            builder.Property(i => i.CreatedAt).HasDefaultValueSql("GETUTCDATE()");

            //RelationShip
            builder.HasOne(i => i.Inspection).WithMany(ins => ins.InspectionResult).HasForeignKey(i => i.InspectionId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(i => i.Rule).WithMany(r => r.InspectionResults).HasForeignKey(i => i.RuleId)
                .OnDelete(DeleteBehavior.Restrict);

            // Indexes
            builder.HasIndex(ir => ir.InspectionId)
                .HasDatabaseName("IX_InspectionResults_InspectionId");

            builder.HasIndex(ir => ir.RuleId)
                .HasDatabaseName("IX_InspectionResults_RuleId");

            builder.HasIndex(ir => new { ir.InspectionId, ir.RuleId })
                .IsUnique()
                .HasDatabaseName("IX_InspectionResults_InspectionId_RuleId");


        }
    }
}

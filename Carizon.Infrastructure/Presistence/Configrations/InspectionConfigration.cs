namespace Carizon.Infrastructure.Presistence.Configrations
{
    public class InspectionConfigration : IEntityTypeConfiguration<Inspection>
    {
        public void Configure(EntityTypeBuilder<Inspection> builder)
        {
            builder.ToTable("Inspections");
            builder.Property(i => i.CarVin).HasMaxLength(17).IsRequired();
            builder.Property(u => u.CarMake).HasMaxLength(50).IsRequired();
            builder.Property(u => u.CarModel).HasMaxLength(50).IsRequired();
            builder.Property(i => i.CarYear).IsRequired();
            builder.Property(i => i.Mileage).IsRequired();
            builder.Property(i => i.CompletedAt).IsRequired(false);
            builder.Property(i => i.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            builder.Property(i => i.Status).HasMaxLength(20).HasConversion<string>().IsRequired().HasDefaultValue(InspectionStatus.pending);

            //RelationShip
            builder.HasOne(i => i.Inspector).WithMany(a => a.Inspectors).HasForeignKey(i => i.InspectorId).OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(i => i.Seller).WithMany(a => a.Sellers).HasForeignKey(i => i.SellerId).OnDelete(DeleteBehavior.NoAction);

            //Index
            builder.HasIndex(i => i.SellerId).HasDatabaseName("IX_Inspections_SellerId");
            builder.HasIndex(i => i.InspectorId).HasDatabaseName("IX_Inspections_InspectorId");
            builder.HasIndex(i => i.Status).HasDatabaseName("IX_Inspections_Status");

        }
    }
}


namespace Carizon.Infrastructure.Presistence.Configrations
{
    public class UserEventConfigration : IEntityTypeConfiguration<UserEvent>
    {
        public void Configure(EntityTypeBuilder<UserEvent> builder)
        {
            builder.ToTable("UserEvents");
            builder.Property(u => u.EventType).HasConversion<string>().HasMaxLength(50).IsRequired();
            builder.Property(u => u.EventData).HasColumnType("nvarchar(max)").IsRequired(false);
            builder.Property(u => u.CreatedAt).HasDefaultValueSql("GETUTCDATE()");


            //RelationShip
            builder.HasOne(u => u.User).WithMany(a => a.UserEvents).HasForeignKey(u => u.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(u => u.Inspection).WithMany(i => i.UserEvents).HasForeignKey(u => u.InspectionId)
                .OnDelete(DeleteBehavior.NoAction);

            //Index
            builder.HasIndex(u => u.UserId).HasDatabaseName("IX_UserEvents_UserId");
            builder.HasIndex(u => u.InspectionId).HasDatabaseName("IX_UserEvents_InspectionId");
            builder.HasIndex(u => u.EventType).HasDatabaseName("IX_UserEvents_EventType");
        }
    }
}

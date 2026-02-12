namespace Carizon.Infrastructure.Presistence.Configrations
{
    public class ApplicationUserConfigrations : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            builder.ToTable("ApplicationUsers");
            builder.HasKey(x => x.Id);
            builder.Property(a => a.FirstName).HasMaxLength(100).IsRequired();
            builder.Property(a => a.LastName).HasMaxLength(100).IsRequired();
            builder.Ignore(a => a.FullName);

            //RelationShip
            builder.HasMany(a => a.RefreshTokens).WithOne(r => r.ApplicationUser).HasForeignKey(r => r.UserId).OnDelete(DeleteBehavior.Cascade);
            builder.HasMany(a => a.UserEvents).WithOne(u => u.User).HasForeignKey(u => u.UserId).OnDelete(DeleteBehavior.NoAction);
        }
    }
}

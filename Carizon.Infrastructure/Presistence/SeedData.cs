namespace Carizon.Infrastructure.Presistence
{
    public static class SeedData
    {
        public static async Task SeedAsync(this IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            // Seed Roles
            await SeedRolesAsync(roleManager);

            // Seed Users
            await SeedUsersAsync(userManager);
        }

        private static async Task SeedRolesAsync(RoleManager<ApplicationRole> roleManager)
        {
            var roles = new[]
            {
                new { Name = "Admin", Description = "Full system access" },
                new { Name = "Inspector", Description = "Inspection operations" },
                new { Name = "Analyst", Description = "Analytics access" },
                new { Name = "Seller", Description = "Car listing management" },
                new { Name = "User", Description = "Basic read access" }
            };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role.Name))
                {
                    await roleManager.CreateAsync(new ApplicationRole
                    {
                        Name = role.Name,
                        Description = role.Description
                    });
                }
            }
        }

        private static async Task SeedUsersAsync(UserManager<ApplicationUser> userManager)
        {
            var users = new[]
            {
                new { Email = "admin@example.com", FirstName = "Admin", LastName = "User", Role = "Admin", Password = "P@ssw0rd" },
                new { Email = "inspector@example.com", FirstName = "Inspector", LastName = "User", Role = "Inspector", Password = "P@ssw0rd" },
                new { Email = "analyst@example.com", FirstName = "Analyst", LastName = "User", Role = "Analyst", Password = "P@ssw0rd" },
                new { Email = "seller@example.com", FirstName = "Seller", LastName = "User", Role = "Seller", Password = "P@ssw0rd" },
                new { Email = "user@example.com", FirstName = "Basic", LastName = "User", Role = "User", Password = "P@ssw0rd" }
            };

            foreach (var user in users)
            {
                var existingUser = await userManager.FindByEmailAsync(user.Email);
                if (existingUser == null)
                {
                    var newUser = new ApplicationUser
                    {
                        UserName = user.Email,
                        Email = user.Email,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        EmailConfirmed = true
                    };

                    var result = await userManager.CreateAsync(newUser, user.Password);
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(newUser, user.Role);
                    }
                }
            }
        }
    }
}

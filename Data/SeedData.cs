using LeaveManagement.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;


namespace LeaveManagement.Data
{
    public static class SeedData
    {
        public static async Task Initialize(IServiceProvider services)
        {
            using var scope = services.CreateScope();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();


            await db.Database.MigrateAsync();


            string[] roles = ["Admin", "Manager", "Employee"];
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));
            }


            // Admin user
            var adminEmail = "admin@leave.local";
            if (await userManager.FindByEmailAsync(adminEmail) is null)
            {
                var admin = new ApplicationUser { UserName = adminEmail, Email = adminEmail, EmailConfirmed = true };
                await userManager.CreateAsync(admin, "Admin@12345!"); // CHANGE ME in production
                await userManager.AddToRoleAsync(admin, "Admin");
            }

            

            // Seed sample leave types
            if (!db.LeaveTypes.Any())
            {
                db.LeaveTypes.AddRange(
                new LeaveType { Name = "Annual", DefaultDays = 20, IsActive = true },
                new LeaveType { Name = "Sick", DefaultDays = 10, IsActive = true }
                );
                await db.SaveChangesAsync();
            }
        }
    }
}
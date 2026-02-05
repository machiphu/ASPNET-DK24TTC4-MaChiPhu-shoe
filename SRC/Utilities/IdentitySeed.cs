using MaChiPhuShoe.Models;
using Microsoft.AspNetCore.Identity;

namespace MaChiPhuShoe.Utilities
{
    public static class IdentitySeed
    {
        public static async Task SeedAsync(IServiceProvider sp)
        {
            var roleMgr = sp.GetRequiredService<RoleManager<IdentityRole>>();
            var userMgr = sp.GetRequiredService<UserManager<AppUser>>();

            // Roles
            string[] roles = { "Admin", "User" };
            foreach (var r in roles)
                if (!await roleMgr.RoleExistsAsync(r))
                    await roleMgr.CreateAsync(new IdentityRole(r));

            // Admin mặc định
            var adminEmail = "admin@machiphushoe.com";
            var admin = await userMgr.FindByEmailAsync(adminEmail);
            if (admin == null)
            {
                admin = new AppUser
                {
                    UserName = "admin",
                    Email = adminEmail,
                    EmailConfirmed = true
                };
                await userMgr.CreateAsync(admin, "Admin@123"); // mk mẫu
            }
            if (!await userMgr.IsInRoleAsync(admin, "Admin"))
                await userMgr.AddToRoleAsync(admin, "Admin");
        }
    }
}

using MaChiPhuShoe.Data;
using MaChiPhuShoe.Models;
using Microsoft.AspNetCore.Identity;

namespace MaChiPhuShoe.Utilities
{
    public class DbInitializer
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<AppUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            // Tên vai trò và email admin
            string adminRoleName = "Admin";
            string adminEmail = "admin@laptopstore.com";

            // 1. Tạo vai trò "Admin" nếu nó chưa tồn tại
            if (!await roleManager.RoleExistsAsync(adminRoleName))
            {
                await roleManager.CreateAsync(new IdentityRole(adminRoleName));
            }

            // 2. Tạo tài khoản admin mẫu nếu chưa có
            if (await userManager.FindByEmailAsync(adminEmail) == null)
            {
                var adminUser = new AppUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true
                };

                // 3. TẠO USER VÀ KIỂM TRA KẾT QUẢ
                // Đây là phần quan trọng nhất được thêm vào
                IdentityResult result = await userManager.CreateAsync(adminUser, "Admin@@123456##");

                if (result.Succeeded)
                {
                    // Nếu tạo user thành công, gán vai trò Admin
                    await userManager.AddToRoleAsync(adminUser, adminRoleName);
                }
                else
                {
                    // NẾU TẠO USER THẤT BẠI, NÉM RA LỖI VỚI MÔ TẢ CHI TIẾT
                    // Lỗi này sẽ hiển thị trên màn hình khi bạn chạy F5
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    throw new Exception($"Không thể tạo tài khoản admin. Lỗi: {errors}");
                }
            }
        }
    }
}

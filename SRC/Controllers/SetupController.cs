using MaChiPhuShoe.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace MaChiPhuShoe.Controllers
{
    public class SetupController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public SetupController(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> Index()
        {
            string adminRoleName = "Admin";
            // THAY ĐỔI EMAIL ADMIN MỚI Ở ĐÂY
            string adminEmail = "machiphu@gmail.com";

            // 1. Tạo vai trò "Admin" nếu nó chưa tồn tại
            if (!await _roleManager.RoleExistsAsync(adminRoleName))
            {
                await _roleManager.CreateAsync(new IdentityRole(adminRoleName));
            }

            // 2. Tìm chính xác người dùng có email là machiphu@gmail.com
            var user = await _userManager.FindByEmailAsync(adminEmail);

            if (user == null)
            {
                ViewBag.Message = $"Không tìm thấy người dùng có email '{adminEmail}'. Vui lòng đăng ký tài khoản này trước.";
                return View();
            }

            // 3. Kiểm tra xem người dùng này đã là Admin chưa
            if (await _userManager.IsInRoleAsync(user, adminRoleName))
            {
                ViewBag.Message = $"Người dùng '{user.Email}' đã là Admin.";
                return View();
            }

            // 4. Nếu chưa, gán vai trò "Admin" cho họ
            await _userManager.AddToRoleAsync(user, adminRoleName);

            ViewBag.Message = $"Thăng chức thành công! Người dùng '{user.Email}' bây giờ đã là Admin.";
            return View();
        }
    }
}

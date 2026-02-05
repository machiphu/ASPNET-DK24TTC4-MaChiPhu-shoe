using MaChiPhuShoe.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace MaChiPhuShoe.Controllers
{
    public class UsersController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UsersController(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // Danh sách khách hàng đã đăng ký
        public IActionResult Index(string? q)
        {
            var users = _userManager.Users.ToList();

            if (!string.IsNullOrWhiteSpace(q))
            {
                users = users.Where(u =>
                           (!string.IsNullOrEmpty(u.UserName) && u.UserName.Contains(q, StringComparison.OrdinalIgnoreCase))
                        || (!string.IsNullOrEmpty(u.Email) && u.Email.Contains(q, StringComparison.OrdinalIgnoreCase))
                       ).ToList();
            }

            var data = users.Select(u => new UserRow
            {
                Id = u.Id,
                UserName = u.UserName,
                Email = u.Email,
                EmailConfirmed = u.EmailConfirmed,
                LockoutEnd = u.LockoutEnd
            }).ToList();

            return View(data);
        }

        [HttpPost]
        public async Task<IActionResult> Lock(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.UtcNow.AddDays(7));
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Unlock(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                await _userManager.SetLockoutEndDateAsync(user, null);
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                await _userManager.DeleteAsync(user);
            }
            return RedirectToAction("Index");
        }
    }

    public class UserRow
    {
        public required string Id { get; init; } // hoặc: public string Id { get; set; } = string.Empty;
        public string? UserName { get; init; }
        public string? Email { get; init; }
        public bool EmailConfirmed { get; init; }
        public DateTimeOffset? LockoutEnd { get; init; }
    }
}

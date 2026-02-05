using MaChiPhuShoe.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; // Cần cho Include()
using System.Security.Claims;
using System.Threading.Tasks;

namespace MaChiPhuShoe.Controllers
{
    [Authorize] // Bắt buộc người dùng phải đăng nhập
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Account/OrderHistory
        public async Task<IActionResult> OrderHistory()
        {
            // Lấy ID của người dùng đang đăng nhập
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Tìm tất cả các đơn hàng của người dùng đó trong database
            // Include(...) để tải kèm thông tin chi tiết (sản phẩm trong đơn hàng)
            var orders = await _context.Orders
                                       .Where(o => o.UserId == userId)
                                       .Include(o => o.OrderDetails)
                                       .ThenInclude(od => od.Product) // Tải kèm thông tin sản phẩm
                                       .OrderByDescending(o => o.OrderDate) // Sắp xếp đơn hàng mới nhất lên đầu
                                       .ToListAsync();

            return View(orders);
        }
    }
}

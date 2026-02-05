using MaChiPhuShoe.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace MaChiPhuShoe.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        
        // GET: /Admin/Dashboard
        public async Task<IActionResult> Dashboard(DateTime? startDate, DateTime? endDate)
        {
            // Mặc định lấy 45 ngày gần nhất nếu không có ngày được chọn
            endDate ??= DateTime.Now;
            startDate ??= endDate.Value.AddDays(-45);

            ViewBag.StartDate = startDate.Value.ToString("yyyy-MM-dd");
            ViewBag.EndDate = endDate.Value.ToString("yyyy-MM-dd");

            // Lọc các đơn hàng trong khoảng thời gian đã chọn
            var ordersInDateRange = _context.Orders
                                            .Where(o => o.OrderDate.Date >= startDate && o.OrderDate.Date <= endDate);

            // Tính toán các chỉ số
            ViewBag.TotalRevenue = await ordersInDateRange.SumAsync(o => o.TotalAmount);
            ViewBag.TotalOrders = await ordersInDateRange.CountAsync();
            ViewBag.TotalProducts = await _context.Products.CountAsync(); // Tổng sản phẩm không đổi
            ViewBag.TotalUsers = await _context.Users.CountAsync(); // Tổng người dùng không đổi

            // Lấy danh sách đơn hàng chi tiết cho lưới
            var detailedOrders = await ordersInDateRange
                                        .Include(o => o.User)
                                        .OrderByDescending(o => o.OrderDate)
                                        .ToListAsync();

            return View(detailedOrders); // Truyền danh sách đơn hàng chi tiết sang View
        }


        // GET: /Admin/OrderList
        public async Task<IActionResult> OrderList()
        {
            var allOrders = await _context.Orders
                                          .Include(o => o.User)
                                          .OrderByDescending(o => o.OrderDate)
                                          .ToListAsync();
            return View(allOrders);
        }

        // GET: /Admin/OrderDetail/5
        public async Task<IActionResult> OrderDetail(int id)
        {
            var order = await _context.Orders
                                      .Include(o => o.User)
                                      .Include(o => o.OrderDetails)
                                      .ThenInclude(od => od.Product)
                                      .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // POST: /Admin/UpdateStatus
        // Action mới để cập nhật trạng thái đơn hàng
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int orderId, string status)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order != null)
            {
                order.Status = status;
                await _context.SaveChangesAsync();
            }
            // Quay lại trang chi tiết của chính đơn hàng đó
            return RedirectToAction("OrderDetail", new { id = orderId });
        }
    }
}

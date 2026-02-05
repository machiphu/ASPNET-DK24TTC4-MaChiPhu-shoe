using MaChiPhuShoe.Data;
using MaChiPhuShoe.Models;
using MaChiPhuShoe.Models.ViewModels; // <-- Thêm dòng này nữa nhé
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text.Json;
using System.Linq; // <-- cần cho Sum, Any,...

namespace MaChiPhuShoe.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private const string CartSessionKey = "MyCart";

        // GET: /Order/Index (Admin)
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            var allOrders = await _context.Orders
                                          .Include(o => o.User)
                                          .OrderByDescending(o => o.OrderDate)
                                          .ToListAsync();
            return View(allOrders);
        }

        public OrderController(ApplicationDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Hiển thị form checkout
        public IActionResult Checkout()
        {
            var cart = GetCartItems();
            if (cart.Count == 0) return RedirectToAction("Index", "Home");

            ViewBag.Cart = cart;
            ViewBag.Total = cart.Sum(item => item.Total);

            // ✅ Khởi tạo đủ các required members để tránh CS9035
            return View(new CheckoutViewModel
            {
                CustomerName = string.Empty,
                ShippingAddress = string.Empty,
                PhoneNumber = string.Empty
            });
        }

        // POST: Nhận dữ liệu từ "tờ đơn" và xử lý
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout(CheckoutViewModel checkoutViewModel)
        {
            var cart = GetCartItems();
            ViewBag.Cart = cart;
            ViewBag.Total = cart.Sum(item => item.Total);

            if (!ModelState.IsValid)
            {
                // Nếu có lỗi, hiển thị lại form với giỏ hiện tại
                return View(checkoutViewModel);
            }

            // Lấy UserId an toàn
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(); // hoặc Challenge();

            // Nếu giỏ rỗng (phòng trường hợp session mất ở tab khác)
            if (cart.Count == 0)
                return RedirectToAction("Index", "Home");

            var order = new Order
            {
                UserId = userId,
                OrderDate = DateTime.Now,
                TotalAmount = cart.Sum(item => item.Total),
                Status = "Đang xử lý",
                CustomerName = checkoutViewModel.CustomerName,
                ShippingAddress = checkoutViewModel.ShippingAddress,
                PhoneNumber = checkoutViewModel.PhoneNumber
            };

            // Đảm bảo danh sách chi tiết được khởi tạo
            order.OrderDetails ??= new List<OrderDetail>();

            foreach (var item in cart)
            {
                order.OrderDetails.Add(new OrderDetail
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    Price = item.Price
                });
            }

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            HttpContext.Session.Remove(CartSessionKey);

            return RedirectToAction("OrderComplete", new { orderId = order.Id });
        }

        public IActionResult OrderComplete(int orderId)
        {
            ViewBag.OrderId = orderId;
            return View();
        }

        // Không bao giờ trả null
        private List<CartItem> GetCartItems()
        {
            var session = HttpContext.Session.GetString(CartSessionKey);
            if (string.IsNullOrEmpty(session))
                return new List<CartItem>();

            return JsonSerializer.Deserialize<List<CartItem>>(session) ?? new List<CartItem>();
        }
    }
}

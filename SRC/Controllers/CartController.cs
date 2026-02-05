using MaChiPhuShoe.Data;
using MaChiPhuShoe.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Text.Json; // Thư viện để làm việc với JSON

namespace MaChiPhuShoe.Controllers
{
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;
        private const string CartSessionKey = "MyCart";

        public CartController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Action để hiển thị trang giỏ hàng
        public IActionResult Index()
        {
            var cart = GetCartItems();
            return View(cart);
        }

        // Action để thêm sản phẩm vào giỏ
        public IActionResult AddToCart(int productId)
        {
            var product = _context.Products.Find(productId);
            if (product == null) return NotFound();

            var cart = GetCartItems();
            var cartItem = cart.Find(p => p.ProductId == productId);

            if (cartItem != null)
            {
                cartItem.Quantity++;
            }
            else
            {
                cart.Add(new CartItem
                {
                    ProductId = product.Id,
                    ProductName = product.Name ?? string.Empty,   // phòng null
                    Price = product.Price,
                    Quantity = 1,
                    ImageUrl = product.ImageUrl ?? string.Empty // phòng null
                });
            }

            SaveCartSession(cart);
            return RedirectToAction("Index");
        }

        // --- CÁC HÀM HỖ TRỢ ---

        // Hàm để lấy giỏ hàng từ Session (không bao giờ trả null)
        private List<CartItem> GetCartItems()
        {
            var session = HttpContext.Session.GetString(CartSessionKey);
            if (string.IsNullOrEmpty(session))
                return new List<CartItem>();

            return JsonSerializer.Deserialize<List<CartItem>>(session) ?? new List<CartItem>();
        }

        // Hàm để lưu giỏ hàng vào Session
        private void SaveCartSession(List<CartItem> cart)
        {
            var session = JsonSerializer.Serialize(cart);
            HttpContext.Session.SetString(CartSessionKey, session);
        }

        // Action để xóa một sản phẩm khỏi giỏ
        public IActionResult RemoveFromCart(int productId)
        {
            var cart = GetCartItems();
            var cartItem = cart.Find(p => p.ProductId == productId);
            if (cartItem != null) cart.Remove(cartItem);

            SaveCartSession(cart);
            return RedirectToAction("Index");
        }

        // Action để tăng số lượng
        public IActionResult AddOne(int productId)
        {
            var cart = GetCartItems();
            var cartItem = cart.Find(p => p.ProductId == productId);
            if (cartItem != null) cartItem.Quantity++;

            SaveCartSession(cart);
            return RedirectToAction("Index");
        }

        // Action để giảm số lượng
        public IActionResult RemoveOne(int productId)
        {
            var cart = GetCartItems();
            var cartItem = cart.Find(p => p.ProductId == productId);
            if (cartItem != null)
            {
                if (cartItem.Quantity > 1) cartItem.Quantity--;
                else cart.Remove(cartItem);
            }

            SaveCartSession(cart);
            return RedirectToAction("Index");
        }
    }
}

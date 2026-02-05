using MaChiPhuShoe.Data;
using MaChiPhuShoe.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace MaChiPhuShoe.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        // Constructor: inject DbContext
        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Trang chủ: hiển thị danh sách sản phẩm
        public IActionResult Index()
        {
            var products = _context.Products.ToList();
            return View(products); // Views/Home/Index.cshtml
        }

        // Tìm kiếm sản phẩm
        public async Task<IActionResult> Search(string query)
        {
            ViewData["SearchQuery"] = query;

            if (string.IsNullOrEmpty(query))
            {
                return View("Index", new List<Product>());
            }

            var products = await _context.Products
                                         .Where(p => p.Name.ToLower().Contains(query.ToLower()))
                                         .ToListAsync();

            return View("Index", products); // Reuse view Index để hiển thị kết quả
        }

        // Trang giới thiệu
        public IActionResult GioiThieu()
        {
            return View(); // Views/Home/GioiThieu.cshtml
        }

        // Trang privacy (có sẵn từ scaffold)
        public IActionResult Privacy()
        {
            return View();
        }
    }
}

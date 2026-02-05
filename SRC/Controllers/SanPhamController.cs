using MaChiPhuShoe.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MaChiPhuShoe.Controllers
{
    public class SanPhamController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SanPhamController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /SanPham
        public async Task<IActionResult> Index(int? category, string? q)
        {
            var queryable = _context.Products.Include(p => p.Category).AsQueryable();

            // Lọc theo category (nếu có)
            if (category.HasValue && category > 0)
            {
                queryable = queryable.Where(p => p.CategoryId == category.Value);
                ViewBag.CategoryId = category.Value;
            }

            // Tìm kiếm theo tên/CPU/RAM (không phân biệt hoa thường)
            if (!string.IsNullOrWhiteSpace(q))
            {
                var keyword = q.Trim();

                queryable = queryable.Where(p =>
                    (p.Name ?? "").Contains(keyword, StringComparison.OrdinalIgnoreCase) //||
                    //(p.Cpu ?? "").Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                    //(p.Ram ?? "").Contains(keyword, StringComparison.OrdinalIgnoreCase)
                );

                ViewBag.Keyword = keyword;
            }

            var products = await queryable
                .OrderByDescending(p => p.Id)
                .ToListAsync();

            return View(products);
        }

        // GET: /SanPham/Details/5
        public async Task<IActionResult> Details(int id)
        {
            if (id <= 0) return BadRequest("Mã sản phẩm không hợp lệ.");

            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null) return NotFound("Không tìm thấy sản phẩm.");

            return View(product);
        }
    }
}

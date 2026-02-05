using MaChiPhuShoe.Data;
using MaChiPhuShoe.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace MaChiPhuShoe.Controllers
{
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductController(ApplicationDbContext context)
        {
            _context = context;
        }

        // HIỂN THỊ DANH SÁCH SẢN PHẨM
        public IActionResult Index()
        {
            var products = _context.Products
                                   .Include(p => p.Category) // load luôn tên Category
                                   .ToList();
            return View(products);
        }

        // CHI TIẾT SẢN PHẨM
        public IActionResult Details(int id)
        {
            var product = _context.Products
                                  .Include(p => p.Category)
                                  .FirstOrDefault(p => p.Id == id);
            if (product == null) return NotFound();
            return View(product);
        }

        // TẠO MỚI (GET)
        public IActionResult Create()
        {
            ViewBag.CategoryId = new SelectList(_context.Categories.OrderBy(c => c.Name), "Id", "Name");
            return View();
        }

        // TẠO MỚI (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.CategoryId = new SelectList(_context.Categories.OrderBy(c => c.Name), "Id", "Name", product.CategoryId);
                return View(product);
            }

            _context.Add(product);
            await _context.SaveChangesAsync();
            TempData["Ok"] = "Đã thêm sản phẩm.";
            return RedirectToAction(nameof(Index));
        }

        // CHỈNH SỬA (GET)
        public async Task<IActionResult> Edit(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();

            ViewBag.CategoryId = new SelectList(_context.Categories.OrderBy(c => c.Name), "Id", "Name", product.CategoryId);
            return View(product);
        }

        // CHỈNH SỬA (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Product product)
        {
            if (id != product.Id) return BadRequest();

            if (!ModelState.IsValid)
            {
                ViewBag.CategoryId = new SelectList(_context.Categories.OrderBy(c => c.Name), "Id", "Name", product.CategoryId);
                return View(product);
            }

            _context.Update(product);
            await _context.SaveChangesAsync();
            TempData["Ok"] = "Đã cập nhật sản phẩm.";
            return RedirectToAction(nameof(Index));
        }

        // XÓA (GET)
        public IActionResult Delete(int id)
        {
            var product = _context.Products
                                  .Include(p => p.Category)
                                  .FirstOrDefault(p => p.Id == id);
            if (product == null) return NotFound();
            return View(product);
        }

        // XÓA (POST)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}

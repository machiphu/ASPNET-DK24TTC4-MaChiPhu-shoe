using MaChiPhuShoe.Data;
using MaChiPhuShoe.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MaChiPhuShoe.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _context;
        public CategoryController(ApplicationDbContext context) => _context = context;

        // GET: /Category
        public async Task<IActionResult> Index()
        {
            var cats = await _context.Categories
                                     .OrderBy(c => c.Name)
                                     .Select(c => new
                                     {
                                         c.Id,
                                         c.Name,
                                         ProductCount = _context.Products.Count(p => p.CategoryId == c.Id)
                                     })
                                     .ToListAsync();

            // map sang viewmodel đơn giản
            var vm = cats.Select(c => new CategoryListItemVM
            {
                Id = c.Id,
                Name = c.Name,
                ProductCount = c.ProductCount
            });

            return View(vm);
        }

        // GET: /Category/Create
        public IActionResult Create() => View(new Category());

        // POST: /Category/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Category model)
        {
            if (!ModelState.IsValid) return View(model);

            _context.Categories.Add(model);
            await _context.SaveChangesAsync();
            TempData["Ok"] = "Đã tạo danh mục.";
            return RedirectToAction(nameof(Index));
        }

        // GET: /Category/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var cat = await _context.Categories.FindAsync(id);
            if (cat == null) return NotFound();
            return View(cat);
        }

        // POST: /Category/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Category model)
        {
            if (id != model.Id) return BadRequest();
            if (!ModelState.IsValid) return View(model);

            _context.Update(model);
            await _context.SaveChangesAsync();
            TempData["Ok"] = "Đã cập nhật danh mục.";
            return RedirectToAction(nameof(Index));
        }

        // GET: /Category/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var cat = await _context.Categories.FindAsync(id);
            if (cat == null) return NotFound();

            var count = await _context.Products.CountAsync(p => p.CategoryId == id);
            ViewBag.ProductCount = count;
            return View(cat);
        }

        // POST: /Category/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id, bool reassign = false)
        {
            var cat = await _context.Categories.FindAsync(id);
            if (cat == null) return NotFound();

            var count = await _context.Products.CountAsync(p => p.CategoryId == id);
            if (count > 0)
            {
                if (!reassign)
                {
                    // không cho xoá khi còn sản phẩm
                    TempData["Err"] = $"Danh mục còn {count} sản phẩm. Hãy chuyển sản phẩm sang danh mục khác trước khi xoá.";
                    return RedirectToAction(nameof(Delete), new { id });
                }

                // chuyển sản phẩm sang "Không phân loại"
                var defaultCat = await _context.Categories
                    .FirstOrDefaultAsync(c => c.Name == "Không phân loại");

                if (defaultCat == null)
                {
                    defaultCat = new Category { Name = "Không phân loại" };
                    _context.Categories.Add(defaultCat);
                    await _context.SaveChangesAsync();
                }

                await _context.Products
                    .Where(p => p.CategoryId == id)
                    .ExecuteUpdateAsync(setters => setters.SetProperty(p => p.CategoryId, defaultCat.Id));
            }

            _context.Categories.Remove(cat);
            await _context.SaveChangesAsync();
            TempData["Ok"] = "Đã xoá danh mục.";
            return RedirectToAction(nameof(Index));
        }
    }

    // ViewModel nhỏ cho Index
    public class CategoryListItemVM
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public int ProductCount { get; set; }
    }
}

using MaChiPhuShoe.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MaChiPhuShoe.Components
{
    public class CategoryMenuViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _db;
        public CategoryMenuViewComponent(ApplicationDbContext db) => _db = db;

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var categories = await _db.Categories.OrderBy(c => c.Name).ToListAsync();
            return View(categories); // tìm ở Views/Shared/Components/CategoryMenu/Default.cshtml
        }
    }
}

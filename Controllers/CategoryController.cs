using Microsoft.AspNetCore.Mvc;
using app_expense_tracker.Models;
using app_expense_tracker.Data;
using Microsoft.EntityFrameworkCore;

namespace app_expense_tracker.Controllers
{
    public class CategoryController : Controller
    {
        private readonly AppDbContext _appDbContext;

        public CategoryController(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<IActionResult> Index()
        {
            return _appDbContext.Categories != null ? View(await _appDbContext.Categories.ToListAsync()) : Problem("Entidad establecida 'AppDbContext.Categories' es null.");
        }

        public IActionResult AddOrEdit(int id = 0)
        {
            if (id == 0)
            {
                return View(new Category());
            }
            else
            {
                return View(_appDbContext.Categories?.Find(id));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddOrEdit([Bind("CategoryId, Title, Icon, Type")] Category category)
        {
            if (ModelState.IsValid)
            {
                if (category.CategoryId == 0)
                {
                    _appDbContext.Add(category);
                }
                else
                {
                    _appDbContext.Update(category);
                }

                await _appDbContext.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            if (_appDbContext.Categories == null)
            {
                return Problem("Entity set 'AppDbContext.Categories' is null.");
            }

            var category = await _appDbContext.Categories.FindAsync(id);

            if (category != null)
            {
                _appDbContext.Categories.Remove(category);
            }

            await _appDbContext.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
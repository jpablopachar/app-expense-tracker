using Microsoft.AspNetCore.Mvc;
using app_expense_tracker.Models;
using app_expense_tracker.Data;
using Microsoft.EntityFrameworkCore;

namespace app_expense_tracker.Controllers
{
    public class TransactionController : Controller
    {
        private readonly AppDbContext _appDbContext;

        public TransactionController(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<IActionResult> Index()
        {
            var appDbContext = _appDbContext.Transactions.Include(transaction => transaction.Category);

            return View(await appDbContext.ToListAsync());
        }

        public IActionResult AddOrEdit(int id = 0)
        {
            PopulateCategories();

            if (id == 0)
            {
                return View(new Transaction());
            }
            else
            {
                return View(_appDbContext.Transactions?.Find(id));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrEdit([Bind("TransactionId, CategoryId, Amount, Note, Date")] Transaction transaction)
        {
            if (ModelState.IsValid)
            {
                if (transaction.TransactionId == 0)
                {
                    _appDbContext.Add(transaction);
                }
                else
                {
                    _appDbContext.Update(transaction);
                }

                await _appDbContext.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            PopulateCategories();

            return View(transaction);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_appDbContext.Transactions == null)
            {
                return Problem("Entidad establecida 'AppDbContext.Transactions' es null.");
            }

            var transaction = await _appDbContext.Transactions.FindAsync(id);

            if (transaction != null)
            {
                _appDbContext.Transactions.Remove(transaction);
            }

            await _appDbContext.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [NonAction]
        public void PopulateCategories()
        {
            var Categories = _appDbContext.Categories?.ToList();

            Category DefaultCategory = new Category()
            {
                CategoryId = 0,
                Title = "Seleccione una categoría"
            };

            Categories?.Insert(0, DefaultCategory);

            ViewBag.Categories = Categories;
        }
    }
}
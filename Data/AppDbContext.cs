using Microsoft.EntityFrameworkCore;
using app_expense_tracker.Models;

namespace app_expense_tracker.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Transaction>? Transactions { get; set; }
        public DbSet<Category>? Categories { get; set; }
    }
}
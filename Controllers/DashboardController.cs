using Microsoft.AspNetCore.Mvc;
using app_expense_tracker.Models;
using app_expense_tracker.Data;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace app_expense_tracker.Controllers
{
    public class DashboardController : Controller
    {
        private readonly AppDbContext _appDbContext;

        public DashboardController(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<IActionResult> Index()
        {
            DateTime StartDate = DateTime.Today.AddDays(-6);
            DateTime EndDate = DateTime.Today;

            List<Transaction> SelectedTransactions = await _appDbContext.Transactions.Include(transaction => transaction.Category).Where(transaction => transaction.Date >= StartDate && transaction.Date <= EndDate).ToListAsync();

            int TotalIncome = SelectedTransactions.Where(transaction => transaction.Category?.Type == "Income").Sum(transaction => transaction.Amount);

            ViewBag.TotalIncome = TotalIncome.ToString("C0");

            int TotalExpense = SelectedTransactions.Where(transaction => transaction.Category?.Type == "Expense").Sum(transaction => transaction.Amount);

            ViewBag.TotalExpense = TotalExpense.ToString("C0");

            int Balance = TotalIncome - TotalExpense;

            CultureInfo cultureInfo = CultureInfo.CreateSpecificCulture("es-ES");

            cultureInfo.NumberFormat.CurrencyNegativePattern = 1;

            ViewBag.Balance = String.Format(cultureInfo, "{0:C0}", Balance);

            ViewBag.DoughnutCharData = SelectedTransactions.Where(transaction => transaction.Category?.Type == "Expense").GroupBy(transaction => transaction.Category?.CategoryId).Select(transaction => new
            {
                categoryTitleWidthIcon = $"{transaction.First().Category?.Icon} {transaction.First().Category?.Title}",
                amount = transaction.Sum(transaction => transaction.Amount),
                formattedAmount = transaction.Sum(transaction => transaction.Amount).ToString("C0")
            }).OrderByDescending(transaction => transaction.amount).ToList();

            List<SplineChartData> IncomeSummary = SelectedTransactions.Where(transaction => transaction.Category?.Type == "Income").GroupBy(transaction => transaction.Date).Select(transaction => new SplineChartData()
            {
                day = transaction.First().Date.ToString("dd MMM"),
                income = transaction.Sum(transaction => transaction.Amount)
            }).ToList();

            List<SplineChartData> ExpenseSummary = SelectedTransactions.Where(transaction => transaction.Category?.Type == "Expense").GroupBy(transaction => transaction.Date).Select(transaction => new SplineChartData()
            {
                day = transaction.First().Date.ToString("dd MMM"),
                expense = transaction.Sum(transaction => transaction.Amount)
            }).ToList();

            string[] Last7Days = Enumerable.Range(0, 7).Select(transaction => DateTime.Today.AddDays(transaction).ToString("dd MMM")).ToArray();

            ViewBag.SplineChartData = from day in Last7Days
                                    join income in IncomeSummary on day equals income.day into dayIncomeJoined
                                    from income in dayIncomeJoined.DefaultIfEmpty()
                                    join expense in ExpenseSummary on day equals expense.day into expenseJoined
                                    from expense in expenseJoined.DefaultIfEmpty()
                                    select new
                                    {
                                        day = day,
                                        income = income == null ? 0 : income.income,
                                        expense = expense == null ? 0 : expense.expense
                                    };

            ViewBag.RecentTransactions = await _appDbContext.Transactions.Include(transaction => transaction.Category).OrderByDescending(transaction => transaction.Date).Take(5).ToListAsync();

            return View();
        }
    }

    public class SplineChartData
    {
        public string? day;
        public int income;
        public int expense;
    }
}
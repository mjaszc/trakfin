using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Trakfin.Data;
using Trakfin.Models;

namespace Trakfin.Controllers
{
    public class ExpensesController : Controller
    {
        private readonly TrakfinContext _context;

        public ExpensesController(TrakfinContext context)
        {
            _context = context;
        }

        // GET: Expenses
        public async Task<IActionResult> Index(string searchString, string bankName, string categoryName, string sortOrder, DateTime? startDate, DateTime? endDate)
        {
            var bankQuery = GetBank();
            var categoryQuery = GetCategory();
            var expenses = FilterExpenses(searchString, bankName, categoryName, startDate, endDate);
            var recurringTransactions = FilterRecurringTransactions(searchString, bankName, categoryName);
            expenses = SortExpenses(expenses, sortOrder);

            var bankNameVM = new ExpenseViewModel
            {
                Expenses = await expenses.ToListAsync(),
                RecurringTransactions = await recurringTransactions.ToListAsync(),
                Banks = new SelectList(await bankQuery.Distinct().ToListAsync()),
                Categories = new SelectList(await categoryQuery.Distinct().ToListAsync()),
            };

            return View(bankNameVM);
        }

        private IQueryable<string> GetCategory() =>
              from e in _context.Expense
              where e.Category != null
              orderby e.Category
              select e.Category;

        private IQueryable<string> GetBank() =>
             from e in _context.Expense
             where e.Bank != null
             orderby e.Bank
             select e.Bank;

        private IQueryable<Expense> GetExpense() =>
            from e in _context.Expense
            select e;
        private IQueryable<Expense> GetRecurringTransactions() =>
            from e in _context.Expense
            where e.Recurring == ExpenseRecurring.Yes
            /*
                It displays only the FIRST result that contains the same properties
                defined in curly braces to avoid duplication when recurring transactions
                are displayed
            */
            group e by new { e.Title, e.Price, e.Category, e.Bank } into x
            select x.FirstOrDefault();

        private IQueryable<Expense> FilterExpenses(string searchString, string bankName, string categoryName, DateTime? startDate, DateTime? endDate)
        {
            var expenses = GetExpense();

            if (!String.IsNullOrEmpty(searchString))
            {
                expenses = expenses.Where(s => s.Title != null && s.Title.Contains(searchString));
            }

            if (!String.IsNullOrEmpty(bankName))
            {
                expenses = expenses.Where(x => x.Bank == bankName);
            }

            if (!String.IsNullOrEmpty(categoryName))
            {
                expenses = expenses.Where(z => z.Category == categoryName);
            }

            if (startDate.HasValue)
            {
                expenses = expenses.Where(x => x.Date >= startDate);
            }

            if (endDate.HasValue)
            {
                expenses = expenses.Where(x => x.Date <= endDate);
            }

            return expenses;
        }

        private IQueryable<Expense> SortExpenses(IQueryable<Expense> expenses, string sortOrder)
        {
            ViewData["TitleSortParm"] = sortOrder == "Title" ? "Title_desc" : "Title";
            ViewData["DateSortParm"] = sortOrder == "Date" ? "Date_desc" : "Date";

            return sortOrder switch
            {
                "Title" => expenses.OrderBy(e => e.Title),
                "Title_desc" => expenses.OrderByDescending(e => e.Title),
                "Date" => expenses.OrderBy(e => e.Date),
                "Date_desc" => expenses.OrderByDescending(e => e.Date),
                _ => expenses.OrderBy(e => e.Id),
            };
        }

        /*
            It is responsible for displaying recurring transaction,
            depending on whether the selected filters fit the recurring transaction properties

            If yes, alongside with filtered expenses, also above it displays recurring transactions
            with matching one or more than one property
        */
        private IQueryable<Expense> FilterRecurringTransactions(string searchString, string bankName, string categoryName)
        {
            var expenses = GetExpense();

            if (!String.IsNullOrEmpty(searchString))
            {
                expenses = expenses.Where(s => s.Title != null && s.Title.Contains(searchString) && s.Recurring == ExpenseRecurring.Yes);
            }

            if (!String.IsNullOrEmpty(bankName))
            {
                expenses = expenses.Where(x => x.Bank == bankName && x.Recurring == ExpenseRecurring.Yes);
            }

            if (!String.IsNullOrEmpty(categoryName))
            {
                expenses = expenses.Where(z => z.Category == categoryName && z.Recurring == ExpenseRecurring.Yes);
            }

            // If all arguments are null
            if (String.IsNullOrEmpty(searchString) && String.IsNullOrEmpty(bankName) && String.IsNullOrEmpty(categoryName))
            {
                expenses = GetRecurringTransactions();
            }

            return expenses;
        }

        [HttpPost]
        public string Index(string searchString, bool notUsed)
        {
            return "From [HttpPost]Index: filter on " + searchString;
        }

        // GET: Expenses/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var expense = await _context.Expense
                .FirstOrDefaultAsync(m => m.Id == id);
            if (expense == null)
            {
                return NotFound();
            }

            return View(expense);
        }

        // GET: Expenses/Create
        public IActionResult Create(string title = "", decimal price = 0, string bank = "", string category = "", ExpensePaymentMethod? paymentMethod = null, ExpenseRecurring? recurring = null)
        {
            // Arguments are passed from Recurring Transaction "Add Transaction" anchor tag,
            // and it is pre-filling the values in the Create Expense page
            var model = new Expense
            {
                Title = title,
                Price = price,
                Bank = bank,
                Category = category,
                PaymentMethod = paymentMethod,
                Date = DateTime.Now,
                Recurring = recurring
            };

            return View(model);
        }

        // POST: Expenses/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Date,Bank,Price,Category,Note,PaymentMethod,Recurring,MerchantOrVendor,Tags,Status")] Expense expense)
        {
            if (ModelState.IsValid)
            {
                _context.Add(expense);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(expense);
        }

        // GET: Expenses/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var expense = await _context.Expense.FindAsync(id);
            if (expense == null)
            {
                return NotFound();
            }
            return View(expense);
        }

        // POST: Expenses/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Date,Bank,Category,Price")] Expense expense)
        {
            if (id != expense.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(expense);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ExpenseExists(expense.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(expense);
        }

        // GET: Expenses/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var expense = await _context.Expense
                .FirstOrDefaultAsync(m => m.Id == id);
            if (expense == null)
            {
                return NotFound();
            }

            return View(expense);
        }

        // POST: Expenses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var expense = await _context.Expense.FindAsync(id);
            if (expense != null)
            {
                _context.Expense.Remove(expense);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ExpenseExists(int id)
        {
            return _context.Expense.Any(e => e.Id == id);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        public async Task<IActionResult> Index(string searchString, string bankName, string categoryName, string sortOrder, DateTime? date = null)
        {
            if (_context.Expense == null)
            {
                return Problem("Entity set 'Trakfin.Expense' is null.");
            }

            var bankQuery = GetBankQuery();
            var categoryQuery = GetCategoryQuery();
            var expenses = FilterExpenses(searchString, bankName, categoryName, date);
            expenses = SortExpenses(expenses, sortOrder);

            var bankNameVM = new ExpenseViewModel
            {
                Expenses = await expenses.ToListAsync(),
                Banks = new SelectList(await bankQuery.Distinct().ToListAsync()),
                Categories = new SelectList(await categoryQuery.Distinct().ToListAsync()),
            };

            return View(bankNameVM);
        }

        private IQueryable<string> GetCategoryQuery() =>
              from e in _context.Expense
              orderby e.Category
              select e.Category;

        private IQueryable<string> GetBankQuery() =>
             from e in _context.Expense
             orderby e.Bank
             select e.Bank;

        private IQueryable<Expense> GetExpenseQuery() =>
            from e in _context.Expense
            select e;

        private IQueryable<Expense> FilterExpenses(string searchString, string bankName, string categoryName, DateTime? date = null)
        {
            var expenses = GetExpenseQuery();

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

            if (date.HasValue)
            {
                expenses = expenses.Where(y => EF.Functions.DateDiffDay(y.Date, date.Value) == 0);
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
        public IActionResult Create()
        {
            return View();
        }

        // POST: Expenses/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Date,Bank,Price,Category,Note")] Expense expense)
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

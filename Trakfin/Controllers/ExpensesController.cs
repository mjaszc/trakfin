using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Text;
using Trakfin.Models;
using CustomFilter = Trakfin.Models.CustomFilter;

namespace Trakfin.Controllers
{
    //[Authorize]
    public class ExpensesController : Controller
    {
        private readonly IConfiguration _config;
        private readonly HttpClient _client;
        private readonly Uri _baseAddress;

        public ExpensesController(IConfiguration config)
        {
            _config = config;
            _baseAddress = new Uri(_config["API_URL"] ?? throw new ArgumentNullException(_config["API_URL"]));
            _client = new HttpClient
            {
                BaseAddress = _baseAddress
            };
        }

        // Class to deserialize the budget details (used in HttpGet Create Method)
        public class BudgetDetail
        {
            public int Id { get; set; }
            public string? NameAndAmount { get; set; }
        }

        // GET: Expenses
        [HttpGet]
        public async Task<IActionResult> Index(string searchString, string bankName, string categoryName, string sortOrder, DateTime? startDate, DateTime? endDate)
        {
            if (startDate.HasValue && endDate.HasValue && startDate > endDate)
            {
                TempData["ErrorMessage"] = "Start date cannot be later than end date.";
                return RedirectToAction(nameof(Index));
            }

            // Tasks to perform before the page loads
            var bankListTask = GetBankNames();
            var categoryListTask = GetCategory(); 
            var expensesTask = FilterExpenses(searchString, bankName, categoryName, startDate, endDate); // Responsible for displaying expenses on the Index page
            var customFiltersTask = GetCustomFilters();
            var recurringTransactionsTask = FilterRecurringTransactions(searchString, bankName, categoryName);
            var budgetNamesTask = GetBudgetNames();

            await Task.WhenAll(bankListTask, categoryListTask, expensesTask, customFiltersTask, recurringTransactionsTask, budgetNamesTask);

            var expensesVm = new ExpenseViewModel
            {
                BankList = new SelectList((await bankListTask).Distinct().ToList()),
                CategoryList = new SelectList((await categoryListTask).Distinct().ToList()),

                Expenses = OrderExpensesByCriteria((await expensesTask).AsQueryable(), sortOrder).ToList(),

                CustomFilters = (await customFiltersTask).ToList(),
                RecurringTransactions = (await recurringTransactionsTask).ToList(),

                // Display names in column called "Budget"
                BudgetNames = await budgetNamesTask,
            };

            return View(expensesVm);
        }

        private async Task<List<string>> GetCategory()
        {
            var response = await _client.GetAsync(_client.BaseAddress + "/Expenses");
            response.EnsureSuccessStatusCode();
            var data = await response.Content.ReadAsStringAsync();
            var expenses = JsonConvert.DeserializeObject<List<Expense>>(data);

            if (expenses == null)
            {
                return [];
            }

            return expenses
                .Where(e => e.Category != null)
                .OrderBy(e => e.Category)
                .Select(e => e.Category!)
                .ToList();
        }

        private async Task<List<string>> GetBankNames()
        {
            var response = await _client.GetAsync(_client.BaseAddress + "/Expenses");
            response.EnsureSuccessStatusCode();
            var data = await response.Content.ReadAsStringAsync();
            var expenses = JsonConvert.DeserializeObject<List<Expense>>(data);

            if (expenses == null)
            {
                return [];
            }

            return expenses
                .Where(e => !string.IsNullOrEmpty(e.Bank))
                .Select(e => e.Bank!)
                .Distinct()
                .ToList();
        }

        private async Task<IQueryable<Expense>> GetExpense()
        {
            var response = await _client.GetAsync(_client.BaseAddress + "/Expenses");
            response.EnsureSuccessStatusCode();
            var data = await response.Content.ReadAsStringAsync();
            var expenses = JsonConvert.DeserializeObject<List<Expense>>(data);

            if (expenses == null)
            {
                return new List<Expense>().AsQueryable();
            }

            return expenses.AsQueryable();
        }

        private async Task<IQueryable<Expense>> GetRecurringTransactions()
        {
            var response = await _client.GetAsync(_client.BaseAddress + "/Expenses");
            response.EnsureSuccessStatusCode();
            var data = await response.Content.ReadAsStringAsync();
            var expenses = JsonConvert.DeserializeObject<List<Expense>>(data);

            if (expenses == null)
            {
                return new List<Expense>().AsQueryable();
            }

            var recurringExpenses = expenses
                .Where(e => e.Recurring == ExpenseRecurring.Yes)
                .GroupBy(e => new { e.Title, e.Price, e.Category, e.Bank })
                .Select(g => g.FirstOrDefault()!)
                .AsQueryable();

            return recurringExpenses;
        }

        private async Task<IQueryable<CustomFilter>> GetCustomFilters()
        {
            var response = await _client.GetAsync(_client.BaseAddress + "/CustomFilters");
            response.EnsureSuccessStatusCode();
            var data = await response.Content.ReadAsStringAsync();
            var filters = JsonConvert.DeserializeObject<List<CustomFilter>>(data);

            if (filters == null)
            {
                return new List<CustomFilter>().AsQueryable();
            }

            return filters.AsQueryable();
        }


        private async Task<Dictionary<int, string>> GetBudgetNames()
        {
            var response = await _client.GetAsync(_client.BaseAddress + "/Expenses");
            response.EnsureSuccessStatusCode();
            var data = await response.Content.ReadAsStringAsync();
            var expenses = JsonConvert.DeserializeObject<List<Expense>>(data);

            if (expenses == null)
            {
                return new Dictionary<int, string>();
            }

            return expenses
                .Where(e => e.Budget != null)
                .ToDictionary(e => e.Id, e => e.Budget!.Name ?? string.Empty);
        }


        private async Task<IQueryable<Expense>> FilterExpenses(string searchString, string bankName, string categoryName, DateTime? startDate, DateTime? endDate)
        {
            var expenses = await GetExpense();

            if (!string.IsNullOrEmpty(searchString))
            {
                expenses = expenses.Where(e => e.Title != null && e.Title.Contains(searchString));
            }

            if (!string.IsNullOrEmpty(bankName))
            {
                expenses = expenses.Where(e => e.Bank == bankName);
            }

            if (!string.IsNullOrEmpty(categoryName))
            {
                expenses = expenses.Where(e => e.Category == categoryName);
            }

            if (startDate.HasValue)
            {
                expenses = expenses.Where(e => e.Date >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                expenses = expenses.Where(e => e.Date <= endDate.Value);
            }

            return expenses.AsQueryable();
        }

        private IQueryable<Expense> OrderExpensesByCriteria(IQueryable<Expense> expenses, string sortOrder)
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
        private async Task<IQueryable<Expense>> FilterRecurringTransactions(string searchString, string bankName, string categoryName)
        {
            var expenses = await GetExpense();

            if (!string.IsNullOrEmpty(searchString))
            {
                expenses = expenses.Where(s => s.Title != null && s.Title.Contains(searchString) && s.Recurring == ExpenseRecurring.Yes);
            }

            if (!string.IsNullOrEmpty(bankName))
            {
                expenses = expenses.Where(x => x.Bank == bankName && x.Recurring == ExpenseRecurring.Yes);
            }

            if (!string.IsNullOrEmpty(categoryName))
            {
                expenses = expenses.Where(z => z.Category == categoryName && z.Recurring == ExpenseRecurring.Yes);
            }

            // If all arguments are null
            if (string.IsNullOrEmpty(searchString) && string.IsNullOrEmpty(bankName) && string.IsNullOrEmpty(categoryName))
            {
                expenses = await GetRecurringTransactions();
            }

            return expenses;
        }

        // GET: Expenses/Details/5
        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                TempData["ErrorMessage"] = "Expense ID is required.";
                return RedirectToAction(nameof(Index));
            }

            Expense? expense = null;
            var response = await _client.GetAsync(_client.BaseAddress + $"/Expenses/{id}");

            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                expense = JsonConvert.DeserializeObject<Expense>(data);
            }
            else
            {
                TempData["ErrorMessage"] = "Expense not found.";
                return RedirectToAction(nameof(Index));
            }

            return View(expense);
        }

        // GET: Expenses/Create
        [HttpGet]
        public async Task<IActionResult> Create(string title = "", decimal price = 0, string bank = "", string category = "", ExpensePaymentMethod? paymentMethod = null, ExpenseRecurring? recurring = null)
        {
            var response = await _client.GetAsync(_client.BaseAddress + "/Budgets");
            List<Budget>? budgets = null;

            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                budgets = JsonConvert.DeserializeObject<List<Budget>>(data);
            }

            if (budgets == null)
            {
                budgets = new List<Budget>();
            }

            var budgetDetails = budgets.Select(b => new BudgetDetail
            {
                Id = b.Id,
                NameAndAmount = $"{b.Name}, {b.BudgetAmount}"
            }).ToList();

            ViewBag.BudgetDetails = new SelectList(budgetDetails, "Id", "NameAndAmount");

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
        public async Task<IActionResult> Create(Expense expense)
        {
            var data = JsonConvert.SerializeObject(expense);

            if (ModelState.IsValid)
            {
                var budgetResponse = await _client.GetAsync(_client.BaseAddress + "/Budgets");

                if (!budgetResponse.IsSuccessStatusCode)
                {
                    TempData["ErrorMessage"] = "Failed to fetch budget data from API.";
                    return RedirectToAction(nameof(Index));
                }

                var budgetData = await budgetResponse.Content.ReadAsStringAsync();
                var budgets = JsonConvert.DeserializeObject<List<Budget>>(budgetData);

                var budgetDetails = budgets!
                    .Where(b => b.Id == expense.BudgetId!.Value)
                    .Select(b => new
                    {
                        b.Id,
                        b.BudgetAmount,
                        b.SpentAmount,
                    })
                    .FirstOrDefault();

                if (budgetDetails != null && budgetDetails!.BudgetAmount > expense.Price)
                {
                    var selectedBudget = budgets!.FirstOrDefault(b => b.Id == expense.BudgetId!.Value);
                    var updatedBudgetAmount = budgetDetails.BudgetAmount - expense.Price;

                    // Updates in the Budget model
                    selectedBudget!.BudgetAmount = updatedBudgetAmount;
                    selectedBudget!.SpentAmount += expense.Price;

                    var updatedBudgetData = JsonConvert.SerializeObject(selectedBudget);
                    StringContent updatedBudgetContent = new(updatedBudgetData, Encoding.UTF8, "application/json");
                    var updatedBudgetEditResponse = await _client.PutAsync(_client.BaseAddress + $"/Budgets/{selectedBudget.Id}", updatedBudgetContent);

                    if (!updatedBudgetEditResponse.IsSuccessStatusCode)
                    {
                        TempData["ErrorMessage"] = "There was an error with modifying budget model.";
                        return RedirectToAction(nameof(Index));
                    }
                }

                StringContent content = new(data, Encoding.UTF8, "application/json");
                var response = await _client.PostAsync(_client.BaseAddress + "/Expenses", content);

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Expense has been successfully created.";
                    return RedirectToAction(nameof(Index));
                }
            }

            return View(expense);
        }

        // GET: Expenses/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                TempData["ErrorMessage"] = "Expense ID is required.";
                return RedirectToAction(nameof(Index));
            }

            var budgetResponse = await _client.GetAsync(_client.BaseAddress + "/Budgets");

            if (!budgetResponse.IsSuccessStatusCode)
            {
                TempData["ErrorMessage"] = "Failed to fetch budget data from API.";
                return RedirectToAction(nameof(Index));
            }

            var budgetData = await budgetResponse.Content.ReadAsStringAsync();
            var budgets = JsonConvert.DeserializeObject<List<Budget>>(budgetData);
            var budgetDetails = budgets!.Select(b => new BudgetDetail
            {
                Id = b.Id,
                NameAndAmount = $"{b.Name}, {b.BudgetAmount}"
            }).ToList();

            ViewBag.BudgetNames = new SelectList(budgetDetails, "Id", "NameAndAmount");

            Expense? expense = null;
            var response = await _client.GetAsync(_client.BaseAddress + $"/Expenses/{id}");
            
            if (!response.IsSuccessStatusCode)
            {
                TempData["ErrorMessage"] = "Expense not found.";
                return RedirectToAction(nameof(Index));
            }

            var data = await response.Content.ReadAsStringAsync();
            expense = JsonConvert.DeserializeObject<Expense>(data);

            if (expense == null)
            {
                TempData["ErrorMessage"] = "Failed to deserialize expense data.";
                return RedirectToAction(nameof(Index));
            }

            return View(expense);
        }

        // POST: Expenses/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Expense expense)
        {
            if (id != expense.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var data = JsonConvert.SerializeObject(expense);
                    StringContent content = new(data, Encoding.UTF8, "application/json");

                    var response = await _client.PutAsync(_client.BaseAddress + $"/Expenses/{id}", content);

                    if (response.IsSuccessStatusCode)
                    {
                        TempData["SuccessMessage"] = "Expense has been successfully modified.";
                        return RedirectToAction(nameof(Index));
                    }

                    TempData["ErrorMessage"] = "Could not find the expense you want to edit.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await ExpenseExists(expense.Id))
                    {
                        return NotFound();
                    }

                    throw;
                }
            }

            return View(expense);
        }

        // GET: Expenses/Delete/5
        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                TempData["ErrorMessage"] = "Please add the ID of the expense you want to delete.";
                return RedirectToAction(nameof(Index));
            }

            Expense? expense = null;
            var response = await _client.GetAsync(_client.BaseAddress + $"/Expenses/{id}");

            if (response.IsSuccessStatusCode)
            {
                var data = response.Content.ReadAsStringAsync().Result;
                expense = JsonConvert.DeserializeObject<Expense>(data);
            }
            else
            {
                TempData["ErrorMessage"] = "Could not find the expense you want to delete.";
                return RedirectToAction(nameof(Index));
            }

            return View(expense);
        }

        // POST: Expenses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (id <= 0)
            {
                TempData["ErrorMessage"] = "Invalid expense ID.";
                return RedirectToAction(nameof(Index));
            }

            var response = await _client.DeleteAsync(_client.BaseAddress + $"/Expenses/{id}");

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Expense has been successfully deleted.";
                return RedirectToAction(nameof(Index));
            }

            return View();
        }

        private async Task<bool> ExpenseExists(int id)
        {
            var response = await _client.GetAsync(_client.BaseAddress + $"/Expenses/{id}");
            return response.IsSuccessStatusCode;
        }
    }
}

using System.Text;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Trakfin.Data;
using Trakfin.Models;

namespace Trakfin.Controllers
{
    public class BudgetsController : Controller
    {
        private readonly Uri _baseAddress = new("https://localhost:7181/api");
        private readonly HttpClient _client;

        public BudgetsController()
        {
            _client = new HttpClient
            {
                BaseAddress = _baseAddress
            };
        }

        // GET: Budgets
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            List<Budget>? budgetList = [];
            var response = await _client.GetAsync(_client.BaseAddress + "/Budgets");

            if (response.IsSuccessStatusCode)
            {
                var data = response.Content.ReadAsStringAsync().Result;
                budgetList = JsonConvert.DeserializeObject<List<Budget>>(data);
            }

            return View(budgetList);
        }

        // GET: Budgets/Details/5
        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Budget? budget = null;
            var response = await _client.GetAsync(_client.BaseAddress + $"/Budgets/{id}");

            if (response.IsSuccessStatusCode)
            {
                var data = response.Content.ReadAsStringAsync().Result;
                budget = JsonConvert.DeserializeObject<Budget>(data);
            }

            return View(budget);
        }

        // GET: Budgets/Create
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Budgets/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Budget budget)
        {
            var data = JsonConvert.SerializeObject(budget);
            StringContent content = new(data, Encoding.UTF8, "application/json");
            var response = await _client.PostAsync(_client.BaseAddress + "/Budgets", content);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }

            return View();
        }

        // GET: Budgets/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Budget? budget = null;
            var response = await _client.GetAsync(_client.BaseAddress + $"/Budgets/{id}");

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                budget = JsonConvert.DeserializeObject<Budget>(data);
            }

            return View(budget);
        }

        // POST: Budgets/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Budget budget)
        {
            if (id != budget.Id)
            {
                return NotFound();
            }

            string data = JsonConvert.SerializeObject(budget);
            StringContent content = new(data, Encoding.UTF8, "application/json");

            var response = await _client.PutAsync(_client.BaseAddress + $"/Budgets/{id}", content);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }

            return View(budget);
        }

        // GET: Budgets/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Budget? budget = null;
            var response = await _client.GetAsync(_client.BaseAddress + $"/Budgets/{id}");

            if (response.IsSuccessStatusCode)
            {
                var data = response.Content.ReadAsStringAsync().Result;
                budget = JsonConvert.DeserializeObject<Budget>(data);
            }

            return View(budget);
        }

        // POST: Budgets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var response = await _client.DeleteAsync(_client.BaseAddress + $"/Budgets/{id}");

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }

            return View();
        }
    }
}

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
        public IActionResult Index()
        {
            List<Budget>? budgetList = new List<Budget>();
            HttpResponseMessage response = _client.GetAsync(_client.BaseAddress + "/Budgets").Result;

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                budgetList = JsonConvert.DeserializeObject<List<Budget>>(data);
            }

            return View(budgetList);
        }

        // GET: Budgets/Details/5
        [HttpGet]
        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Budget? budget = null;
            HttpResponseMessage response = _client.GetAsync(_client.BaseAddress + $"/Budgets/{id}").Result;

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
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
        public IActionResult Create(Budget budget)
        {
            string data = JsonConvert.SerializeObject(budget);
            StringContent content = new StringContent(data, Encoding.UTF8, "application/json");
            HttpResponseMessage response = _client.PostAsync(_client.BaseAddress + "/Budgets", content).Result;

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }

            return View();
        }

        // GET: Budgets/Edit/5
        [HttpGet]
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Budget? budget = null;
            HttpResponseMessage response = _client.GetAsync(_client.BaseAddress + $"/Budgets/{id}").Result;

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
        public IActionResult Edit(int id, Budget budget)
        {
            if (id != budget.Id)
            {
                return NotFound();
            }

            string data = JsonConvert.SerializeObject(budget);
            StringContent content = new StringContent(data, Encoding.UTF8, "application/json");

            HttpResponseMessage response = _client.PutAsync(_client.BaseAddress + $"/Budgets/{id}", content).Result;

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }

            return View(budget);
        }

        // GET: Budgets/Delete/5
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Budget? budget = null;
            HttpResponseMessage response = _client.GetAsync(_client.BaseAddress + $"/Budgets/{id}").Result;

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                budget = JsonConvert.DeserializeObject<Budget>(data);
            }

            return View(budget);
        }

        // POST: Budgets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            HttpResponseMessage response = _client.DeleteAsync(_client.BaseAddress + $"/Budgets/{id}").Result;

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }

            return View();
        }
    }
}

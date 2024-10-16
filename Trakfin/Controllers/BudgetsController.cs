using System.Net.Http;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Trakfin.Models;

namespace Trakfin.Controllers
{
    public class BudgetsController : Controller
    {
        private readonly IConfiguration _config;
        private readonly HttpClient _client;
        private readonly Uri _baseAddress;
        private readonly ILogger<BudgetsController> _logger;

        public BudgetsController(IConfiguration config, ILogger<BudgetsController> logger)
        {
            _config = config;
            _baseAddress = new Uri(_config["API_URL"] ?? throw new ArgumentNullException(_config["API_URL"]));
            _client = new HttpClient
            {
                BaseAddress = _baseAddress
            };
            _logger = logger;
        }

        // GET: Budgets
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            List<Budget>? budgetList = [];
            try
            {
                HttpClient client = new();
                client.Timeout = TimeSpan.FromMinutes(3);

                _logger.LogInformation("Starting to get budgets from API.");
                var response = await client.GetAsync(_baseAddress + "/Budgets");

                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    budgetList = JsonConvert.DeserializeObject<List<Budget>>(data);
                    _logger.LogInformation($"Successfully retrieved {budgetList!.Count} budgets");
                }
                else
                {
                    _logger.LogWarning($"Failed to get budgets from API. Status code: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while getting budgets from API {ex.Message}");
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

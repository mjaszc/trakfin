using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Text;
using CustomFilter = Trakfin.Models.CustomFilter;

namespace Trakfin.Controllers
{
    public class CustomFiltersController : Controller
    {
        private readonly IConfiguration _config;
        private readonly HttpClient _client;
        private readonly Uri _baseAddress;

        public CustomFiltersController(IConfiguration config)
        {
            _config = config;
            _baseAddress = new Uri(_config["API_URL"] ?? throw new ArgumentNullException(_config["API_URL"]));
            _client = new HttpClient
            {
                BaseAddress = _baseAddress
            };
        }

        // GET: CustomFilters
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            List<CustomFilter>? filterList = [];
            var response = await _client.GetAsync(_client.BaseAddress + "/CustomFilters");

            if (response.IsSuccessStatusCode)
            {
                var data = response.Content.ReadAsStringAsync().Result;
                filterList = JsonConvert.DeserializeObject<List<CustomFilter>>(data);
            }

            return View(filterList);
        }

        // GET: CustomFilters/Details/5
        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            CustomFilter? filter = null;
            var response = await _client.GetAsync(_client.BaseAddress + $"/CustomFilters/{id}");

            if (response.IsSuccessStatusCode)
            {
                var data = response.Content.ReadAsStringAsync().Result;
                filter = JsonConvert.DeserializeObject<CustomFilter>(data);
            }

            return View(filter);
        }

        // GET: CustomFilters/Create
        [HttpGet]
        public IActionResult Create(string bankName, string categoryName, DateTime? startDate, DateTime? endDate, string searchString)
        {
            var model = new CustomFilter
            {
                Bank = bankName,
                Category = categoryName,
                StartDate = startDate,
                EndDate = endDate,
                Title = searchString // Assuming you want to use the SearchString as the Title
            };

            return View(model);
        }

        // POST: CustomFilters/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CustomFilter customFilter)
        {
            var data = JsonConvert.SerializeObject(customFilter);
            StringContent content = new(data, Encoding.UTF8, "application/json");
            var response = await _client.PostAsync(_client.BaseAddress + "/CustomFilters", content);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }

            return View();
        }

        // GET: CustomFilters/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            CustomFilter? budget = null;
            var response = await _client.GetAsync(_client.BaseAddress + $"/CustomFilters/{id}");

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                budget = JsonConvert.DeserializeObject<CustomFilter>(data);
            }

            return View(budget);
        }

        // POST: CustomFilters/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CustomFilter customFilter)
        {
            if (id != customFilter.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var data = JsonConvert.SerializeObject(customFilter);
                    StringContent content = new(data, Encoding.UTF8, "application/json");

                    var response = await _client.PutAsync(_client.BaseAddress + $"/CustomFilters/{id}", content);

                    if (response.IsSuccessStatusCode)
                    {
                        return RedirectToAction(nameof(Index));
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await CustomFilterExists(customFilter.Id))
                    {
                        return NotFound();
                    }

                    throw;
                }
            }

            return View(customFilter);
        }

        // GET: CustomFilters/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            CustomFilter? filter = null;
            var response = await _client.GetAsync(_client.BaseAddress + $"/CustomFilters/{id}");

            if (response.IsSuccessStatusCode)
            {
                var data = response.Content.ReadAsStringAsync().Result;
                filter = JsonConvert.DeserializeObject<CustomFilter>(data);
            }

            return View(filter);
        }

        // POST: CustomFilters/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var response = await _client.DeleteAsync(_client.BaseAddress + $"/CustomFilters/{id}");

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }

            return View();
        }

        private async Task<bool> CustomFilterExists(int id)
        {
            var response = await _client.GetAsync(_client.BaseAddress + $"/CustomFilters/{id}");
            return response.IsSuccessStatusCode;
        }
    }
}

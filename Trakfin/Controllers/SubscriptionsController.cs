using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Trakfin.Data;
using Trakfin.Models;

namespace Trakfin.Controllers
{
    public class SubscriptionsController : Controller
    {
        private readonly IConfiguration _config;
        private readonly HttpClient _client;
        private readonly Uri _baseAddress;

        public SubscriptionsController(IConfiguration config)
        {
            _config = config;
            _baseAddress = new Uri(_config["API_URL"] ?? throw new ArgumentNullException(_config["API_URL"]));
            _client = new HttpClient
            {
                BaseAddress = _baseAddress
            };
        }


        // GET: Subscriptions
        public async Task<IActionResult> Index(string searchString)
        {
            var subscriptions = await FilterSubscriptions(searchString);
            var subscriptionsList = subscriptions.ToList();
            var subscriptionsVm = new SubscriptionViewModel { Subscriptions = subscriptionsList };

            return View(subscriptionsVm);
        }

        private async Task<IQueryable<Subscription>> GetSubscriptions()
        {
            var response = await _client.GetAsync(_client.BaseAddress + "/Subscriptions");
            response.EnsureSuccessStatusCode();
            var data = await response.Content.ReadAsStringAsync();
            var subscriptions = JsonConvert.DeserializeObject<List<Subscription>>(data);

            if (subscriptions == null)
            {
                return new List<Subscription>().AsQueryable();
            }

            return subscriptions.AsQueryable();
        }

        private async Task<IQueryable<Subscription>> FilterSubscriptions(string searchString)
        {
            var subscriptions = await GetSubscriptions();

            if (!string.IsNullOrEmpty(searchString))
            {
                subscriptions = subscriptions.Where(s => s.Name!.Contains(searchString, StringComparison.OrdinalIgnoreCase));
            }

            return subscriptions;
        }

        // GET: Subscriptions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Subscription? sub = null;
            var response = await _client.GetAsync(_client.BaseAddress + $"/Subscriptions/{id}");

            if (response.IsSuccessStatusCode)
            {
                var data = response.Content.ReadAsStringAsync().Result;
                sub = JsonConvert.DeserializeObject<Subscription>(data);
            }

            return View(sub);
        }

        // GET: Subscriptions/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Subscriptions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Subscription subscription)
        {
            var data = JsonConvert.SerializeObject(subscription);
            StringContent content = new(data, Encoding.UTF8, "application/json");
            var response = await _client.PostAsync(_client.BaseAddress + "/Subscriptions", content);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }

            return View();
        }

        // GET: Subscriptions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Subscription? sub = null;
            var response = await _client.GetAsync(_client.BaseAddress + $"/Subscriptions/{id}");

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                sub = JsonConvert.DeserializeObject<Subscription>(data);
            }

            return View(sub);
        }

        // POST: Subscriptions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Subscription subscription)
        {
            if (id != subscription.Id)
            {
                return NotFound();
            }

            string data = JsonConvert.SerializeObject(subscription);
            StringContent content = new(data, Encoding.UTF8, "application/json");

            var response = await _client.PutAsync(_client.BaseAddress + $"/Subscriptions/{id}", content);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }

            return View(subscription);
        }

        // GET: Subscriptions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Subscription? sub = null;
            var response = await _client.GetAsync(_client.BaseAddress + $"/Subscriptions/{id}");

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                sub = JsonConvert.DeserializeObject<Subscription>(data);
            }

            return View(sub);
        }

        // POST: Subscriptions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var response = await _client.DeleteAsync(_client.BaseAddress + $"/Subscriptions/{id}");

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }

            return View();
        }
    }
}

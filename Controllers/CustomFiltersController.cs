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
    public class CustomFiltersController : Controller
    {
        private readonly TrakfinContext _context;

        public CustomFiltersController(TrakfinContext context)
        {
            _context = context;
        }

        // GET: CustomFilters
        public async Task<IActionResult> Index()
        {
            return View(await _context.CustomFilter.ToListAsync());
        }

        // GET: CustomFilters/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customFilter = await _context.CustomFilter
                .FirstOrDefaultAsync(m => m.Id == id);
            if (customFilter == null)
            {
                return NotFound();
            }

            return View(customFilter);
        }

        // GET: CustomFilters/Create
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
        public async Task<IActionResult> Create([Bind("Id,Bank,Category,StartDate,EndDate,Title")] CustomFilter customFilter)
        {
            if (ModelState.IsValid)
            {
                _context.Add(customFilter);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(customFilter);
        }

        // GET: CustomFilters/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customFilter = await _context.CustomFilter.FindAsync(id);
            if (customFilter == null)
            {
                return NotFound();
            }
            return View(customFilter);
        }

        // POST: CustomFilters/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Bank,Category,StartDate,EndDate,Title")] CustomFilter customFilter)
        {
            if (id != customFilter.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(customFilter);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CustomFilterExists(customFilter.Id))
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
            return View(customFilter);
        }

        // GET: CustomFilters/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customFilter = await _context.CustomFilter
                .FirstOrDefaultAsync(m => m.Id == id);
            if (customFilter == null)
            {
                return NotFound();
            }

            return View(customFilter);
        }

        // POST: CustomFilters/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var customFilter = await _context.CustomFilter.FindAsync(id);
            if (customFilter != null)
            {
                _context.CustomFilter.Remove(customFilter);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CustomFilterExists(int id)
        {
            return _context.CustomFilter.Any(e => e.Id == id);
        }
    }
}

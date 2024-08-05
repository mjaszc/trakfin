using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrakfinAPI.Data;
using TrakfinAPI.Models;

namespace TrakfinAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomFiltersController : ControllerBase
    {
        private readonly TrakfinAPIContext _context;

        public CustomFiltersController(TrakfinAPIContext context)
        {
            _context = context;
        }

        // GET: api/CustomFilters
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CustomFilter>>> GetCustomFilter()
        {
            return await _context.CustomFilter.ToListAsync();
        }

        // GET: api/CustomFilters/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CustomFilter>> GetCustomFilter(int id)
        {
            var customFilter = await _context.CustomFilter.FindAsync(id);

            if (customFilter == null)
            {
                return NotFound();
            }

            return customFilter;
        }

        // PUT: api/CustomFilters/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCustomFilter(int id, CustomFilter customFilter)
        {
            if (id != customFilter.Id)
            {
                return BadRequest();
            }

            _context.Entry(customFilter).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CustomFilterExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/CustomFilters
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<CustomFilter>> PostCustomFilter(CustomFilter customFilter)
        {
            _context.CustomFilter.Add(customFilter);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCustomFilter", new { id = customFilter.Id }, customFilter);
        }

        // DELETE: api/CustomFilters/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomFilter(int id)
        {
            var customFilter = await _context.CustomFilter.FindAsync(id);
            if (customFilter == null)
            {
                return NotFound();
            }

            _context.CustomFilter.Remove(customFilter);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CustomFilterExists(int id)
        {
            return _context.CustomFilter.Any(e => e.Id == id);
        }
    }
}

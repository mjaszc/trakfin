using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrakfinAPI.Data;
using TrakfinAPI.Models;

namespace TrakfinAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BudgetsController(TrakfinAPIContext context) : ControllerBase
    {
        // GET: api/Budgets
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Budget>>> GetBudget()
        {
            return await context.Budget.ToListAsync();
        }

        // GET: api/Budgets/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Budget>> GetBudget(int id)
        {
            var budget = await context.Budget.FindAsync(id);

            if (budget == null)
            {
                return NotFound();
            }

            return budget;
        }

        // PUT: api/Budgets/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBudget(int id, Budget budget)
        {
            if (id != budget.Id)
            {
                return BadRequest();
            }

            context.Entry(budget).State = EntityState.Modified;

            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BudgetExists(id))
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

        // POST: api/Budgets
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Budget>> PostBudget(Budget budget)
        {
            context.Budget.Add(budget);
            await context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetBudget), new { id = budget.Id }, budget);
        }

        // DELETE: api/Budgets/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBudget(int id)
        {
            var budget = await context.Budget.FindAsync(id);
            if (budget == null)
            {
                return NotFound();
            }

            context.Budget.Remove(budget);
            await context.SaveChangesAsync();

            return NoContent();
        }

        private bool BudgetExists(int id)
        {
            return context.Budget.Any(e => e.Id == id);
        }
    }
}

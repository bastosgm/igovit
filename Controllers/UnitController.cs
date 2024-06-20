using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using igovit.Models;
using Microsoft.AspNetCore.Authorization;

namespace igovit.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UnitController : ControllerBase
    {
        private readonly UnitContext _context;

        public UnitController(UnitContext context)
        {
            _context = context;
        }

        // GET: api/unit
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Unit>>> GetUnits()
        {
            var unitsWithEmployees = await _context.Units
            .Include(f => f.Employees)
            .ToListAsync();

            return unitsWithEmployees;
        }

        // GET: api/unit/:id
        [HttpGet("{id}")]
        public async Task<ActionResult<Unit>> GetUnit(Guid id)
        {
            var unit = await _context.Units
                .Include(f => f.Employees)
                .FirstOrDefaultAsync(f => f.Id == id);

            if (unit == null) return NotFound();

            return unit;
        }

        // PUT: api/unit/:id
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUnit(Guid id, [FromBody] Unit unit)
        {
            if (unit.Id != Guid.Empty || unit.Name != null)
            {
                return BadRequest("Unexpected fields. 'Id' and/or 'Name' cannot be modified.");
            }

            var existingUnit = await _context.Units.FindAsync(id);

            if (existingUnit == null) return NotFound();

            if (unit.Status != null) {
                if (unit.Status != "active" && unit.Status != "inactive") {
                    return BadRequest("Unexpected status value. Must be 'active' or 'inactive' only.");
                }
                existingUnit.Status = unit.Status;
            }

            _context.Entry(existingUnit).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UnitExists(id))
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

        // POST: api/unit
        [HttpPost]
        public async Task<ActionResult<Unit>> PostUnit([FromBody] Unit unit)
        {
            if (unit.Name == null) return BadRequest("Name field must be provided.");

            if (unit.Status == null) return BadRequest("Status field must be provided.");

            if (
                unit.Status != "ativo" &&
                unit.Status != "inativo" &&
                unit.Status != "active" &&
                unit.Status != "inactive")
            {
                return BadRequest("Unexpected status value. Must be 'ativo/active' or 'inativo/inactive'.");
            }

            if (unit.Status == "ativo") unit.Status = "active";

            if (unit.Status == "inativo") unit.Status = "inactive";

            unit.Employees = [];

            _context.Units.Add(unit);

            await _context.SaveChangesAsync();

            return CreatedAtAction("Getunit", new { id = unit.Id }, unit);
        }

        // Method commented on because it was not mentioned in the challenge
        //
        // DELETE: api/unit/:id
        // [HttpDelete("{id}")]
        // public async Task<IActionResult> Deleteunit(Guid id)
        // {
        //     var unit = await _context.units.FindAsync(id);
        //     if (unit == null)
        //     {
        //         return NotFound();
        //     }

        //     _context.units.Remove(unit);
        //     await _context.SaveChangesAsync();

        //     return NoContent();
        // }

        private bool UnitExists(Guid id)
        {
            return _context.Units.Any(e => e.Id == id);
        }
    }
}

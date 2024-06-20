using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using igovit.Models;
using Microsoft.AspNetCore.Authorization;

namespace igovit.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class EmployeeController : ControllerBase
    {
        private readonly EmployeeContext _context;

        public EmployeeController(EmployeeContext context)
        {
            _context = context;
        }

        // GET: api/employee
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EmployeeDto>>> GetEmployees()
        {
            var employees = await _context.Employees.ToListAsync();

            var employeesListResponse = employees.Select(employee => new EmployeeDto
            {
                EmployeeId = employee.EmployeeId,
                Name = employee.Name,
                Unit = employee.Unit,
            }).ToList();

            return employeesListResponse;
        }

        // GET: api/employee/:id
        [HttpGet("{id}")]
        public async Task<ActionResult<EmployeeDto>> GetEmployee(Guid id)
        {
            var employee = await _context.Employees.FindAsync(id);

            if (employee == null)
            {
                return NotFound();
            }

            var employeeDto = new EmployeeDto
            {
                EmployeeId = employee.EmployeeId,
                Name = employee.Name,
                Unit = employee.Unit,
            };

            return employeeDto;
        }

        // PUT: api/employee/:id
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEmployee(Guid id, Employee employee)
        {
            if (employee.UserId != Guid.Empty )
            {
                return BadRequest("Unexpected fields. 'UserId' cannot be modified.");
            }

            var existingEmployee = await _context.Employees.FindAsync(id);

            if (existingEmployee == null) return NotFound("There is no Employee with this id");

            existingEmployee.Name = employee.Name;
            existingEmployee.Unit = employee.Unit;

            _context.Entry(existingEmployee).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EmployeeExists(id))
                {
                    return NotFound("ff");
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/employee
        [HttpPost]
        public async Task<ActionResult<Employee>> PostEmployee([FromBody] Employee employee)
        {
            if (employee.Name == null) return BadRequest("Name field must be provided.");

            if (employee.Unit == null) return BadRequest("Unit field must be provided.");

            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "UserId");

            if (userIdClaim == null) return BadRequest("UserId does not exist.");

            if (!Guid.TryParse(userIdClaim.Value, out Guid userId)) return BadRequest("Invalid UserId.");

            if (_context.Employees.Any(e => e.UserId == userId))
            {
                return BadRequest("An Employee is already linked to this User. Try a different User token at create a new Employee");
            }

            var existingUnit = await _context.Units.Include(f => f.Employees).SingleOrDefaultAsync(f => f.Name == employee.Unit);

            if (existingUnit == null) return BadRequest("Unit not found.");

            if (existingUnit.Status == "inactive")
            {
                return BadRequest("Cannot create an employee for an inactive Unit.");
            }

            employee.UserId = userId;

            existingUnit.Employees?.Add(employee);

            await _context.SaveChangesAsync();

            return CreatedAtAction("GetEmployee", new { id = employee.EmployeeId }, employee);
        }

        // DELETE: api/employee/:id
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployee(Guid id)
        {
            var employee = await _context.Employees.FindAsync(id);

            if (employee == null) return NotFound();

            _context.Employees.Remove(employee);
            
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool EmployeeExists(Guid id)
        {
            return _context.Employees.Any(e => e.EmployeeId == id);
        }
    }
}

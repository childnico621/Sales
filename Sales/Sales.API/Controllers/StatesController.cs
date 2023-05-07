using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sales.API.Data;
using Sales.API.Helper;
using Sales.Shared.DTO;
using Sales.Shared.Entities;

namespace Sales.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatesController : ControllerBase
    {
        private readonly DataContext _context;

        public StatesController(DataContext dataContext) => _context = dataContext;


        [HttpGet]
        public async Task<IActionResult> GetAsync([FromQuery] PaginationDto pagination)
        {
            var querable = _context.States.Include(c => c.Cities).Where(x => x.Country!.Id == pagination.Id).AsQueryable();

            if (!string.IsNullOrWhiteSpace(pagination.Filter))
            {
                querable = querable.Where(c => c.Name.ToLower().Contains(pagination.Filter.ToLower()));
            }

            return Ok(await querable
               .OrderBy(c => c.Name)
               .Paginate(pagination)
               .ToListAsync());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAsync(int id)
        {
            var state = await _context.States
                .Include(c => c.Cities)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (state == null)
            {
                return NotFound();
            }

            return Ok(state);
        }

        [HttpGet("[action]")]
        public async Task<ActionResult> GetPages([FromQuery] PaginationDto pagination)
        {
            var querable = _context.States.Where(x => x.Country!.Id == pagination.Id).AsQueryable();

            if (!string.IsNullOrWhiteSpace(pagination.Filter))
            {
                querable = querable.Where(c => c.Name.ToLower().Contains(pagination.Filter.ToLower()));
            }

            double count = await querable.CountAsync();
            double totalPages = Math.Ceiling(count / pagination.RecordNumber);
            return Ok(totalPages);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var state = await _context.States.FirstOrDefaultAsync(c => c.Id == id);
            if (state == null)
            {
                return NotFound();
            }
            _context.Remove(state);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPost]
        public async Task<IActionResult> PostAsync(State state)
        {
            _context.Add(state);

            try
            {
                await _context.SaveChangesAsync();
                return Ok(state);
            }
            catch (DbUpdateException dbUpdateException)
            {
                if (dbUpdateException.InnerException!.Message.Contains("duplicate"))
                    return BadRequest("Ya existe un país con el mismo nombre.");

                return BadRequest(dbUpdateException.Message);

            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut]
        public async Task<IActionResult> PutAsync(State state)
        {
            _context.Update(state);
            try
            {
                await _context.SaveChangesAsync();
                return Ok(state);
            }
            catch (DbUpdateException dbUpdateException)
            {
                if (dbUpdateException.InnerException!.Message.Contains("duplicate"))
                    return BadRequest("Ya existe un país con el mismo nombre.");

                return BadRequest(dbUpdateException.Message);

            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
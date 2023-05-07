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
    public class CountriesController : ControllerBase
    {
        private readonly DataContext _context;
        public CountriesController(DataContext dataContext) => _context = dataContext;


        [HttpGet]
        public async Task<IActionResult> GetAsync([FromQuery] PaginationDto pagination)
        {
            var querable = _context.Countries.Include(c => c.States).AsQueryable();

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
            var country = await _context.Countries
                .Include(c => c.States!)
                .ThenInclude(s => s.Cities)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (country == null) return NotFound();


            return Ok(country);
        }

        [HttpGet("[action]")]
        public async Task<ActionResult> GetPages([FromQuery] PaginationDto pagination)
        {
            var querable = _context.Countries.AsQueryable();

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
            var country = await _context.Countries.FirstOrDefaultAsync(c => c.Id == id);
            if (country == null)
            {
                return NotFound();
            }
            _context.Remove(country);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPost]
        public async Task<IActionResult> PostAsync(Country country)
        {
            _context.Add(country);

            try
            {
                await _context.SaveChangesAsync();
                return Ok(country);
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
        public async Task<IActionResult> PutAsync(Country country)
        {
            _context.Update(country);
            try
            {
                await _context.SaveChangesAsync();
                return Ok(country);
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

using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Commissionor.WebApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Commissionor.WebApi.Controllers
{
    [Route("api/[controller]")]
    public class ReadersController : Controller
    {
        readonly CommissionorDbContext dbContext;

        public ReadersController(CommissionorDbContext dbContext) {
            this.dbContext = dbContext;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var allReaders = await dbContext.Readers
                                            .Include(r => r.Locations)
                                            .ToListAsync();
            return Ok(allReaders);
        }

        [HttpGet("{readerId}")]
        public async Task<IActionResult> Get(string readerId)
        {
            if (string.IsNullOrWhiteSpace(readerId))
                return BadRequest();

            var reader  = await dbContext.Readers
                                         .Include(r => r.Locations)
                                         .SingleOrDefaultAsync(r => r.Id == readerId);
            
            return reader != null ? (IActionResult)Ok(reader) : NotFound();
        }

        [HttpPut("{readerId}")]
        public async Task<IActionResult> Create(string readerId, [FromBody] Reader reader)
        {
            if (string.IsNullOrWhiteSpace(readerId) || reader == null)
                return BadRequest();

            if (!string.IsNullOrWhiteSpace(reader.Id) && reader.Id != readerId)
                return BadRequest();

            ModelState.Clear();
            reader.Id = readerId;
            if (!TryValidateModel(reader))
                return BadRequest(ModelState);

            var readerExists = await dbContext.Readers.AnyAsync(r => r.Id == reader.Id);
            if (readerExists)
                return StatusCode((int)HttpStatusCode.Conflict);

            await dbContext.Readers.AddAsync(reader);
            await dbContext.SaveChangesAsync();
            return Ok();
        }

        [HttpPost("{readerId}/locations")]
        public async Task<IActionResult> AddLocation(string readerId, [FromBody] Location location)
        {
            if (string.IsNullOrWhiteSpace(readerId) || location == null)
                return BadRequest();

            if (!string.IsNullOrWhiteSpace(location.ReaderId) && location.ReaderId != readerId)
                return BadRequest();

            ModelState.Clear();
            location.ReaderId = readerId;
            if (!TryValidateModel(location))
                return BadRequest(ModelState);

            var readerExists = await dbContext.Readers.AnyAsync(r => r.Id == readerId);
            if (!readerExists)
                return NotFound();

            dbContext.Locations.Add(location);
            await dbContext.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete("{readerId}")]
        public async Task<IActionResult> Delete(string readerId)
        {
            if (string.IsNullOrWhiteSpace(readerId))
                return BadRequest();

            var reader = await dbContext.Readers
                                        .Include(r => r.Locations)
                                        .SingleOrDefaultAsync(r => r.Id == readerId);

            if (reader != null)
            {
                dbContext.Readers.Remove(reader);
                await dbContext.SaveChangesAsync();
                return Ok();
            }
            return NotFound();
        }
    }
}

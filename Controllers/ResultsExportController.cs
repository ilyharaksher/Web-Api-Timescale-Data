using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web_Api_Timescale_Data.Data;
using Web_Api_Timescale_Data.DTO;
using Web_Api_Timescale_Data.Entities;



namespace Web_Api_Timescale_Data.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ResultsExportController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;
        public ResultsExportController(ApplicationDbContext dbContext)
        {
            this._dbContext = dbContext;
        }

        [HttpGet]
        public async Task<IActionResult> GetResults([FromQuery] ResultFilterDTO filter)
        {
            IQueryable<ResultEntity> query = _dbContext.Results;
            if (filter.FileName != null)
            {
                query = query.Where(x => x.FileName == filter.FileName);
            }
            if (filter.MinStartDate.HasValue)
            {
                query = query.Where(x => x.MinDate >= filter.MinStartDate);
            }
            if (filter.MaxStartDate.HasValue)
            {
                query = query.Where(x => x.MinDate <= filter.MaxStartDate);
            }
            if (filter.MinAvgValue.HasValue)
            {
                query = query.Where(x => x.AvgValue >= filter.MinAvgValue);
            }
            if (filter.MaxAvgValue.HasValue)
            {
                query = query.Where(x => x.AvgValue <= filter.MaxAvgValue);
            }
            if (filter.MinAvgTime.HasValue)
            {
                query = query.Where(x => x.AvgExecutionTime >= filter.MinAvgTime);
            }
            if (filter.MaxAvgTime.HasValue)
            {
                query = query.Where(x => x.AvgExecutionTime <= filter.MaxAvgTime);
            }

            var results = await query.ToListAsync();

            return Ok(results);
        }
    }
}

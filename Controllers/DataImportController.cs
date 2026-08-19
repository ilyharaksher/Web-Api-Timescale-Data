using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Web_Api_Timescale_Data.Data;
using Web_Api_Timescale_Data.Exceptions;
using Web_Api_Timescale_Data.Services;

namespace Web_Api_Timescale_Data.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DataImportController : ControllerBase
    {
        private readonly ICsvImportService _csvImportService;
        public DataImportController(ICsvImportService csvImportService)
        {
            this._csvImportService = csvImportService;
        }

        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile? file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("Файла нет или он пустой");
            }
            try
            {
                await _csvImportService.ImportAsync(file);
            }
            catch (CsvValidationException ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok("Файл загружен!");
        }
    }
}

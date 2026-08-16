namespace Web_Api_Timescale_Data.Services
{
    public interface ICsvImportService
    {
        Task ImportAsync(IFormFile file);
    }
}

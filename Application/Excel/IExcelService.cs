using Microsoft.AspNetCore.Http;

namespace Infrastructure.Services.Excel;

public interface IExcelService
{
    Task<List<Dictionary<string, string>>> ReadAsync(IFormFile file);
}
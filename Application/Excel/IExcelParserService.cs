using Microsoft.AspNetCore.Http;

namespace Application.Excel;

public interface IExcelParserService
{
    Task<ExcelParseResult> ParseAsync(IFormFile file);
}
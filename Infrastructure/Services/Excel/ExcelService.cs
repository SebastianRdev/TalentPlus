using OfficeOpenXml;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.Services.Excel;

public class ExcelService : IExcelService
{
    public async Task<List<Dictionary<string, string>>> ReadAsync(IFormFile file)
    {
        using var stream = new MemoryStream();
        await file.CopyToAsync(stream);

        using var package = new ExcelPackage(stream);
        var worksheet = package.Workbook.Worksheets[0];

        var headers = new List<string>();
        var result = new List<Dictionary<string, string>>();

        for (int col = 1; col <= worksheet.Dimension.Columns; col++)
            headers.Add(worksheet.Cells[1, col].Text.Trim());

        for (int row = 2; row <= worksheet.Dimension.Rows; row++)
        {
            var dict = new Dictionary<string, string>();

            for (int col = 1; col <= headers.Count; col++)
                dict[headers[col - 1]] = worksheet.Cells[row, col].Text;

            result.Add(dict);
        }

        return result;
    }
}
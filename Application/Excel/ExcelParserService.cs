using Application.Excel;
using Microsoft.AspNetCore.Http;
using OfficeOpenXml;

namespace Application.Excel;

public class ExcelParserService : IExcelParserService
{
    public async Task<ExcelParseResult> ParseAsync(IFormFile file)
    {
        if (file == null || file.Length == 0)
            throw new InvalidOperationException("The Excel file is empty.");

        using var stream = new MemoryStream();
        await file.CopyToAsync(stream);

        using var package = new ExcelPackage(stream);
        var worksheet = package.Workbook.Worksheets.FirstOrDefault();

        if (worksheet == null)
            throw new InvalidOperationException("The Excel file does not contain sheets.");

        var columnCount = worksheet.Dimension.End.Column;
        var rowCount = worksheet.Dimension.End.Row;

        // Leer headers
        var headers = new List<string>();
        for (int col = 1; col <= columnCount; col++)
        {
            headers.Add(worksheet.Cells[1, col].Text.Trim());
        }

        var rows = new List<Dictionary<string, string>>();

        // Leer filas
        for (int row = 2; row <= rowCount; row++)
        {
            var rowData = new Dictionary<string, string>();

            for (int col = 1; col <= columnCount; col++)
            {
                var header = headers[col - 1];
                var value = worksheet.Cells[row, col].Text.Trim();

                rowData[header] = value;
            }

            rows.Add(rowData);
        }

        return new ExcelParseResult
        {
            Headers = headers,
            Rows = rows
        };
    }
}
namespace Application.Excel;

public class ExcelParseResult
{
    public IReadOnlyList<string> Headers { get; init; } = [];
    public IReadOnlyList<Dictionary<string, string>> Rows { get; init; } = [];
}
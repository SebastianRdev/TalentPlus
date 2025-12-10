using Application.DTOs.Excel;

namespace Application.DTOs.Excel;

public class ExcelPreviewResultDto
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public ExcelPreviewData Data { get; set; } = new();
}

public class ExcelPreviewData
{
    public int TotalValid { get; set; }
    public int TotalInvalid { get; set; }
    public List<ExcelRowPreview> ValidRows { get; set; } = new();
    public List<ExcelRowPreview> InvalidRows { get; set; } = new();
}

public class ExcelRowPreview
{
    public int RowNumber { get; set; }
    public Dictionary<string, string> RowData { get; set; } = new();
    public List<string> Errors { get; set; } = new();
}

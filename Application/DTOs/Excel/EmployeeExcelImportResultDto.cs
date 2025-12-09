namespace Application.DTOs.Excel;

public class EmployeeExcelImportResultDto
{
    public int TotalRows { get; set; }
    public int Created { get; set; }
    public int Updated { get; set; }
    public List<string> Errors { get; set; } = new List<string>();
}

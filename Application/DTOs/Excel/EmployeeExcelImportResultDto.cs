using System.Linq;

namespace Application.DTOs.Excel;

public class EmployeeExcelImportResultDto
{
    public int TotalRows { get; set; }
    public int Created { get; set; }
    public int Updated { get; set; }
    public List<string> Errors { get; set; } = new List<string>();
    
    // Properties to match frontend expectations
    public int InsertedCount => Created + Updated;
    public List<object> FailedRows => Errors.Select((error, index) => new { rowNumber = index + 1, error }).Cast<object>().ToList();
}

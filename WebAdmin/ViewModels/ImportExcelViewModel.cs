namespace WebAdmin.ViewModels;

public class ImportExcelViewModel
{
    public bool Success { get; set; }
    public int Created { get; set; }
    public int Updated { get; set; }
    public List<string> Errors { get; set; } = [];
}
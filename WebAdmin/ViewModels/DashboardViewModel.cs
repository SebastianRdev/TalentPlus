namespace WebAdmin.ViewModels;

/// <summary>
/// ViewModel for the dashboard, containing summary statistics.
/// </summary>
public class DashboardViewModel
{
    /// <summary>
    /// Gets or sets the total number of TotalEmployees.
    /// </summary>
    public int TotalEmployees { get; set; }

    /// <summary>
    /// Gets or sets the total number of EmployeesOnVacation.
    /// </summary>
    public int EmployeesOnVacation { get; set; }

    /// <summary>
    /// Gets or sets the total ActiveEmployees.
    /// </summary>
    public int ActiveEmployees { get; set; }
}
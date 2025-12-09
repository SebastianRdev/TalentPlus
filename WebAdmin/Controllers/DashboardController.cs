using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebAdmin.ViewModels;

namespace WebAdmin.Controllers;

/// <summary>
/// Controller for the main dashboard view.
/// </summary>
[Authorize(Roles = "Admin")]
public class DashboardController : Controller
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DashboardController"/> class.
    /// </summary>
    public DashboardController()
    {
    }

    /// <summary>
    /// Displays the dashboard index view with summary data.
    /// </summary>
    /// <returns>The dashboard view.</returns>
    public IActionResult Index()
    {
        // Datos temporales (mock)
        var model = new DashboardViewModel
        {
            TotalEmployees = 0,
            EmployeesOnVacation = 0,
            ActiveEmployees = 0
        };

        return View(model);
    }
}
using Application.Interfaces.Employees;
using Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/employees")]
[Authorize(Roles = "Admin")]
public class EmployeesController : ControllerBase
{
    private readonly IEmployeeService _employeeService;

    public EmployeesController(IEmployeeService employeeService)
    {
        _employeeService = employeeService;
    }

    [HttpPost("import-excel")]
    public async Task<IActionResult> ImportExcel(IFormFile file)
    {
        if (file is null || file.Length == 0)
            return BadRequest("Archivo Excel requerido.");

        if (!file.FileName.EndsWith(".xlsx"))
            return BadRequest("Formato inválido. Solo se permite .xlsx");

        var result = await _employeeService.ImportFromExcelAsync(file);

        return Ok(new
        {
            message = "Importación finalizada",
            result.Inserted,
            result.Updated
        });
    }
}
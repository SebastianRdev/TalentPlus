using Application.Interfaces.Employees;
using Application.Services;
using Application.DTOs.Empleados;
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

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var employees = await _employeeService.GetAllAsync();
        return Ok(employees);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var employee = await _employeeService.GetByIdAsync(id);
        if (employee == null)
            return NotFound();
        return Ok(employee);
    }

    [HttpPost]
    public async Task<IActionResult> Create(EmployeeCreateDto dto)
    {
        await _employeeService.CreateAsync(dto);
        return Created("", null);
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register(EmployeeRegisterDto dto)
    {
        await _employeeService.RegisterAsync(dto);
        return Ok(new { message = "Registro exitoso. Se ha enviado un correo de confirmación." });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, EmployeeUpdateDto dto)
    {
        try
        {
            await _employeeService.UpdateAsync(id, dto);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            await _employeeService.DeleteAsync(id);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(ex.Message);
        }
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
            result.Created,
            result.Updated,
            result.Errors
        });
    }
}
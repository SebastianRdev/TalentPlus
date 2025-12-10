using Application.Common;
using Application.DTOs.Empleados;
using Application.DTOs.Excel;
using Application.Excel;
using Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace Application.Interfaces.Employees;

public interface IEmployeeService
{
    Task CreateAsync(EmployeeCreateDto dto);
    Task RegisterAsync(EmployeeRegisterDto dto);
    Task UpdateAsync(Guid id, EmployeeUpdateDto dto);
    Task<Empleado> GetByDocumentAsync(string document);
    Task<IEnumerable<Empleado>> GetAllAsync();
    Task<Empleado?> GetByIdAsync(Guid id);
    Task DeleteAsync(Guid id);
    Task<EmployeeExcelImportResultDto> ImportFromExcelAsync(IFormFile file);
}
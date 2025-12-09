using Application.DTOs.Empleados;
using Microsoft.AspNetCore.Http;

namespace Application.Interfaces;

public interface IEmployeeExcelImportService
{
    Task<EmployeeExcelImportResultDto> ImportAsync(IFormFile file);
}
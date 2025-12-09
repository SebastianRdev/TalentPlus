using Application.DTOs.Empleados;
using Application.DTOs.Excel;
using Microsoft.AspNetCore.Http;

namespace Application.Interfaces;

public interface IEmployeeExcelImportService
{
    Task<EmployeeExcelImportResultDto> ImportAsync(IFormFile file);
}
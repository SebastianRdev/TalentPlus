using Application.Common;
using Application.Excel;
using Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace Application.Interfaces.Employees;

public interface IEmployeeService
{
    Task<PagedResult<Empleado>> GetAllAsync(int page, int pageSize);

    Task<Empleado?> GetByIdAsync(Guid id);

    Task CreateAsync(Empleado employee);

    Task UpdateAsync(Empleado employee);

    Task DeleteAsync(Guid id);
    
    Task<ExcelImportResult> ImportFromExcelAsync(IFormFile file);
}
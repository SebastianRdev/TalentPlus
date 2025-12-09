using Application.Common;
using Application.Excel;
using Application.Interfaces.Employees;
using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Services.Excel;
using Microsoft.AspNetCore.Http;

namespace Application.Services;

public class EmployeeService : IEmployeeService
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IExcelService _excelService;

    public EmployeeService(
        IEmployeeRepository employeeRepository,
        IExcelService excelService)
    {
        _employeeRepository = employeeRepository;
        _excelService = excelService;
    }

    public async Task<PagedResult<Empleado>> GetAllAsync(
        int page = 1,
        int pageSize = 10)
    {
        if (page <= 0) page = 1;
        if (pageSize <= 0) pageSize = 10;

        var (items, totalCount) =
            await _employeeRepository.GetPagedAsync(page, pageSize);

        return new PagedResult<Empleado>
        {
            Items = items,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<Empleado?> GetByIdAsync(Guid id)
        => await _employeeRepository.GetByIdAsync(id);

    public async Task CreateAsync(Empleado employee)
    {
        var existing =
            await _employeeRepository.GetByDocumentAsync(employee.Documento);

        if (existing is not null)
            throw new InvalidOperationException(
                "There is already an employee with that document.");

        await _employeeRepository.AddAsync(employee);
    }

    public async Task UpdateAsync(Empleado employee)
        => await _employeeRepository.UpdateAsync(employee);

    public async Task DeleteAsync(Guid id)
    {
        var employee = await _employeeRepository.GetByIdAsync(id);

        if (employee is null)
            throw new KeyNotFoundException("Employee not found.");

        await _employeeRepository.DeleteAsync(employee);
    }
    
    public async Task<ExcelImportResult> ImportFromExcelAsync(IFormFile file)
    {
        var rows = await _excelService.ReadAsync(file);

        int inserted = 0;
        int updated = 0;

        foreach (var row in rows)
        {
            var documento = row["Documento"];

            var existingEmployee =
                await _employeeRepository.GetByDocumentAsync(documento);

            if (existingEmployee is null)
            {
                var employee = Empleado.CreateFromExcel(row);
                await _employeeRepository.AddAsync(employee);
                inserted++;
            }
            else
            {
                existingEmployee.UpdateFromExcel(row);
                await _employeeRepository.UpdateAsync(existingEmployee);
                updated++;
            }
        }

        return new ExcelImportResult
        {
            Inserted = inserted,
            Updated = updated
        };
    }
}
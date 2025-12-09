using Application.Common;
using Application.Interfaces.Employees;
using Domain.Entities;
using Domain.Interfaces;

namespace Application.Services;

public class EmployeeService : IEmployeeService
{
    private readonly IEmployeeRepository _repository;

    public EmployeeService(IEmployeeRepository repository)
    {
        _repository = repository;
    }

    public async Task<PagedResult<Empleado>> GetAllAsync(
        int page = 1,
        int pageSize = 10)
    {
        if (page <= 0) page = 1;
        if (pageSize <= 0) pageSize = 10;

        var (items, totalCount) =
            await _repository.GetPagedAsync(page, pageSize);

        return new PagedResult<Empleado>
        {
            Items = items,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<Empleado?> GetByIdAsync(Guid id)
        => await _repository.GetByIdAsync(id);

    public async Task CreateAsync(Empleado employee)
    {
        var existing =
            await _repository.GetByDocumentAsync(employee.Documento);

        if (existing is not null)
            throw new InvalidOperationException(
                "Ya existe un empleado con ese documento.");

        await _repository.AddAsync(employee);
    }

    public async Task UpdateAsync(Empleado employee)
        => await _repository.UpdateAsync(employee);

    public async Task DeleteAsync(Guid id)
    {
        var employee = await _repository.GetByIdAsync(id);

        if (employee is null)
            throw new KeyNotFoundException("Empleado no encontrado.");

        await _repository.DeleteAsync(employee);
    }
}
using Application.Common;
using Domain.Entities;

namespace Application.Interfaces.Employees;

public interface IEmployeeService
{
    Task<PagedResult<Empleado>> GetAllAsync(int page, int pageSize);

    Task<Empleado?> GetByIdAsync(Guid id);

    Task CreateAsync(Empleado employee);

    Task UpdateAsync(Empleado employee);

    Task DeleteAsync(Guid id);
}
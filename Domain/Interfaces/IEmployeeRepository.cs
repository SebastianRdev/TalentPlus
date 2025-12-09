using Domain.Entities;

namespace Domain.Interfaces;

public interface IEmployeeRepository
{
    Task<Empleado?> GetByIdAsync(Guid id);
    Task<Empleado?> GetByDocumentAsync(string document);

    Task<(IReadOnlyList<Empleado> Items, int TotalCount)> 
        GetPagedAsync(int page, int pageSize);

    Task AddAsync(Empleado employee);
    Task UpdateAsync(Empleado employee);
    Task DeleteAsync(Empleado employee);
}
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class EmployeeRepository : IEmployeeRepository
{
    private readonly ApplicationDbContext _context;

    public EmployeeRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Empleado?> GetByIdAsync(Guid id)
    {
        return await _context.Empleados
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<Empleado?> GetByDocumentAsync(string document)
    {
        return await _context.Empleados
            .FirstOrDefaultAsync(e => e.Documento == document);
    }

    public async Task<IReadOnlyList<Empleado>> GetAllAsync()
    {
        return await _context.Empleados
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<(IReadOnlyList<Empleado> Items, int TotalCount)>
        GetPagedAsync(int page, int pageSize)
    {
        var query = _context.Empleados.AsNoTracking();

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderBy(e => e.Apellidos)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task AddAsync(Empleado employee)
    {
        await _context.Empleados.AddAsync(employee);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Empleado employee)
    {
        _context.Empleados.Update(employee);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Empleado employee)
    {
        _context.Empleados.Remove(employee);
        await _context.SaveChangesAsync();
    }
}
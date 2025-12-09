using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Persistence;
using Application.DTOs.Empleados;
using Microsoft.AspNetCore.Authorization;
using WebAdmin.ViewModels.Employees;
using Domain.Entities;

namespace WebAdmin.Controllers;

public class EmployeesController : Controller
{
    private readonly ApplicationDbContext _context;

    public EmployeesController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var employees = await _context.Empleados
            .Select(e => new EmployeeViewModel
            {
                Id = e.Id,
                Documento = e.Documento,
                Nombres = e.Nombres,
                Apellidos = e.Apellidos,
                Email = e.Email,
                Cargo = e.Cargo.ToString(),
                Departamento = e.Departamento.ToString(),
                Estado = e.Estado.ToString(),
                FechaIngreso = e.FechaIngreso
            })
            .ToListAsync();

        return View(employees);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(EmployeeCreateViewModel viewModel)
    {
        if (!ModelState.IsValid)
            return View(viewModel);
        
        try 
        {
            var employee = Empleado.Create(
                viewModel.Documento,
                viewModel.Nombres,
                viewModel.Apellidos,
                viewModel.FechaNacimiento,
                viewModel.Direccion ?? "",
                viewModel.Telefono ?? "",
                viewModel.Email,
                viewModel.Cargo,
                viewModel.Salario,
                viewModel.FechaIngreso,
                viewModel.Estado,
                viewModel.NivelEducativo,
                viewModel.PerfilProfesional ?? "",
                viewModel.Departamento
            );

            _context.Empleados.Add(employee);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", ex.Message);
            return View(viewModel);
        }
    }

    public async Task<IActionResult> Edit(Guid id)
    {
        var employee = await _context.Empleados.FindAsync(id);
        if (employee == null)
            return NotFound();

        var dto = new EmployeeUpdateDto
        {
            Nombres = employee.Nombres,
            Apellidos = employee.Apellidos,
            FechaNacimiento = employee.FechaNacimiento,
            Direccion = employee.Direccion,
            Telefono = employee.Telefono,
            Email = employee.Email,
            Cargo = employee.Cargo,
            Salario = employee.Salario,
            FechaIngreso = employee.FechaIngreso,
            Estado = employee.Estado,
            NivelEducativo = employee.NivelEducativo,
            PerfilProfesional = employee.PerfilProfesional,
            Departamento = employee.Departamento
        };
        
        var viewModel = new EmployeeUpdateViewModel
        {
            Id = employee.Id,
            Documento = employee.Documento,
            Nombres = employee.Nombres,
            Apellidos = employee.Apellidos,
            FechaNacimiento = employee.FechaNacimiento ?? DateTime.MinValue,
            Direccion = employee.Direccion,
            Telefono = employee.Telefono,
            Email = employee.Email,
            Cargo = employee.Cargo,
            Salario = employee.Salario ?? 0,
            FechaIngreso = employee.FechaIngreso ?? DateTime.MinValue,
            Estado = employee.Estado,
            NivelEducativo = employee.NivelEducativo,
            PerfilProfesional = employee.PerfilProfesional,
            Departamento = employee.Departamento
        };

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, EmployeeUpdateViewModel viewModel)
    {
        if (id != viewModel.Id)
            return NotFound();

        if (!ModelState.IsValid)
            return View(viewModel);

        var employee = await _context.Empleados.FindAsync(id);
        if (employee == null)
            return NotFound();

        try
        {
            employee.Update(
                viewModel.Nombres,
                viewModel.Apellidos,
                viewModel.FechaNacimiento,
                viewModel.Direccion ?? "",
                viewModel.Telefono ?? "",
                viewModel.Email,
                viewModel.Cargo,
                viewModel.Salario,
                viewModel.FechaIngreso,
                viewModel.Estado,
                viewModel.NivelEducativo,
                viewModel.PerfilProfesional ?? "",
                viewModel.Departamento
            );

            _context.Update(employee);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", ex.Message);
            return View(viewModel);
        }
    }

    public async Task<IActionResult> Delete(Guid id)
    {
        var employee = await _context.Empleados.FindAsync(id);
        if (employee == null)
            return NotFound();

        var viewModel = new EmployeeViewModel
        {
            Id = employee.Id,
            Documento = employee.Documento,
            Nombres = employee.Nombres,
            Apellidos = employee.Apellidos,
            Email = employee.Email,
            Cargo = employee.Cargo.ToString(),
            Departamento = employee.Departamento.ToString(),
            Estado = employee.Estado.ToString(),
            FechaIngreso = employee.FechaIngreso
        };

        return View(viewModel);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(Guid id)
    {
        var employee = await _context.Empleados.FindAsync(id);
        if (employee != null)
        {
            _context.Empleados.Remove(employee);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }
}

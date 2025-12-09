using Domain.Enums;

namespace WebAdmin.ViewModels.Employees;

public class EmployeeViewModel
{
    public Guid Id { get; set; }
    public string Documento { get; set; } = default!;
    public string Nombres { get; set; } = default!;
    public string Apellidos { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string Cargo { get; set; } = default!;
    public string Departamento { get; set; } = default!;
    public string Estado { get; set; } = default!;
    public DateTime? FechaIngreso { get; set; }
    public string FullName => $"{Nombres} {Apellidos}";
}
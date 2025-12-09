using Domain.Enums;

namespace Application.DTOs.Empleados;

public class EmployeeResponseDto
{
    public Guid Id { get; set; }
    public string Documento { get; set; } = default!;
    public string Nombres { get; set; } = default!;
    public string Apellidos { get; set; } = default!;
    public string Email { get; set; } = default!;
    public Cargo Cargo { get; set; }
    public EstadoEmpleado Estado { get; set; }
    public Departamento Departamento { get; set; }
}
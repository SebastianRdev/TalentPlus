using Domain.Enums;

namespace Domain.Entities;

public class Empleado
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Documento { get; set; } = default!;
    public string Nombres { get; set; } = default!;
    public string Apellidos { get; set; } = default!;

    public DateTime? FechaNacimiento { get; set; }
    public string? Direccion { get; set; }
    public string? Telefono { get; set; }
    public string Email { get; set; } = default!;

    public Cargo Cargo { get; set; }
    public decimal? Salario { get; set; }
    public DateTime? FechaIngreso { get; set; }

    public EstadoEmpleado Estado { get; set; }
    public NivelEducativo NivelEducativo { get; set; }

    public string? PerfilProfesional { get; set; }
    public Departamento Departamento { get; set; }

    // Link with Identity
    public string? ApplicationUserId { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}
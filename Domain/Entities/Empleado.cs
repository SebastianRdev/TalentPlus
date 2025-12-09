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
    
    private Empleado() { } // EF Core

    public static Empleado Create(
        string documento,
        string nombres,
        string apellidos,
        DateTime fechaNacimiento,
        string direccion,
        string telefono,
        string email,
        Cargo cargo,
        decimal salario,
        DateTime fechaIngreso,
        EstadoEmpleado estado,
        NivelEducativo nivelEducativo,
        string perfilProfesional,
        Departamento departamento)
    {
        ValidateEnums(cargo, estado, nivelEducativo, departamento);

        return new Empleado
        {
            Documento = documento,
            Nombres = nombres,
            Apellidos = apellidos,
            FechaNacimiento = DateTime.SpecifyKind(fechaNacimiento, DateTimeKind.Utc),
            Direccion = direccion,
            Telefono = telefono,
            Email = email,
            Cargo = cargo,
            Salario = salario,
            FechaIngreso = DateTime.SpecifyKind(fechaIngreso, DateTimeKind.Utc),
            Estado = estado,
            NivelEducativo = nivelEducativo,
            PerfilProfesional = perfilProfesional,
            Departamento = departamento
        };
    }

    public void Update(
        string nombres,
        string apellidos,
        DateTime fechaNacimiento,
        string direccion,
        string telefono,
        string email,
        Cargo cargo,
        decimal salario,
        DateTime fechaIngreso,
        EstadoEmpleado estado,
        NivelEducativo nivelEducativo,
        string perfilProfesional,
        Departamento departamento)
    {
        ValidateEnums(cargo, estado, nivelEducativo, departamento);

        Nombres = nombres;
        Apellidos = apellidos;
        FechaNacimiento = DateTime.SpecifyKind(fechaNacimiento, DateTimeKind.Utc);
        Direccion = direccion;
        Telefono = telefono;
        Email = email;
        Cargo = cargo;
        Salario = salario;
        FechaIngreso = DateTime.SpecifyKind(fechaIngreso, DateTimeKind.Utc);
        Estado = estado;
        NivelEducativo = nivelEducativo;
        PerfilProfesional = perfilProfesional;
        Departamento = departamento;
        UpdatedAt = DateTime.UtcNow;
    }

    private static void ValidateEnums(
        Cargo cargo,
        EstadoEmpleado estado,
        NivelEducativo nivel,
        Departamento departamento)
    {
        if (cargo == Cargo.Unknown)
            throw new InvalidOperationException("Invalid cargo.");

        if (estado == EstadoEmpleado.Unknown)
            throw new InvalidOperationException("Invalid state.");

        if (nivel == NivelEducativo.Unknown)
            throw new InvalidOperationException("Invalid educational level.");

        if (departamento == Departamento.Unknown)
            throw new InvalidOperationException("Invalid department.");
    }
}
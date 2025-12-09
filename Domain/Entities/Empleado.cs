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
    
    private static string GetRequired(
        Dictionary<string, string> row,
        string key)
    {
        if (!row.TryGetValue(key, out var value) ||
            string.IsNullOrWhiteSpace(value))
            throw new InvalidOperationException($"{key} is required.");

        return value.Trim();
    }

    private static DateTime GetDate(
        Dictionary<string, string> row,
        string key)
    {
        var value = GetRequired(row, key);

        if (!DateTime.TryParse(value, out var date))
            throw new InvalidOperationException($"{key} It does not have a valid format.");

        return date;
    }

    private static decimal GetDecimal(
        Dictionary<string, string> row,
        string key)
    {
        var value = GetRequired(row, key)
            .Replace(",", "."); // Excel LATAM

        if (!decimal.TryParse(value, out var number))
            throw new InvalidOperationException($"{key} It is not a valid number.");

        return number;
    }

    private static Cargo ParseCargo(string value)
    {
        var normalized = Normalize(value);

        return normalized switch
        {
            "ingeniero" => Cargo.Ingeniero,
            "soportetecnico" => Cargo.SoporteTecnico,
            "coordinador" => Cargo.Coordinador,
            "analista" => Cargo.Analista,
            "desarrollador" => Cargo.Desarrollador,
            "administrador" => Cargo.Administrador,
            "auxiliar" => Cargo.Auxiliar,
            _ => Cargo.Unknown
        };
    }

    private static EstadoEmpleado ParseEstado(string value)
    {
        var normalized = Normalize(value);

        return normalized switch
        {
            "activo" => EstadoEmpleado.Activo,
            "vacaciones" => EstadoEmpleado.Vacaciones,
            "inactivo" => EstadoEmpleado.Inactivo,
            _ => EstadoEmpleado.Unknown
        };
    }

    private static NivelEducativo ParseNivelEducativo(string value)
    {
        var normalized = Normalize(value);

        return normalized switch
        {
            "tecnico" => NivelEducativo.Tecnico,
            "tecnologo" => NivelEducativo.Tecnologo,
            "profesional" => NivelEducativo.Profesional,
            "especializacion" => NivelEducativo.Especializacion,
            "maestria" => NivelEducativo.Maestria,
            _ => NivelEducativo.Unknown
        };
    }

    private static Departamento ParseDepartamento(string value)
    {
        var normalized = Normalize(value);

        return normalized switch
        {
            "recursoshumanos" or "rrhh" => Departamento.RecursosHumanos,
            "tecnologia" => Departamento.Tecnologia,
            "operaciones" => Departamento.Operaciones,
            "logistica" => Departamento.Logistica,
            "marketing" => Departamento.Marketing,
            "ventas" => Departamento.Ventas,
            "contabilidad" => Departamento.Contabilidad,
            _ => Departamento.Unknown
        };
    }

    private static string Normalize(string value) =>
        value.Trim().ToLower()
            .Replace(" ", "")
            .Replace("_", "")
            .Replace("-", "");

    public static Empleado CreateFromExcel(
        Dictionary<string, string> row)
    {
        var cargo = ParseCargo(GetRequired(row, "Cargo"));
        var estado = ParseEstado(GetRequired(row, "Estado"));
        var nivel = ParseNivelEducativo(GetRequired(row, "NivelEducativo"));
        var departamento = ParseDepartamento(GetRequired(row, "Departamento"));

        ValidateEnums(cargo, estado, nivel, departamento);

        return new Empleado
        {
            Id = Guid.NewGuid(),
            Documento = GetRequired(row, "Documento"),
            Nombres = GetRequired(row, "Nombres"),
            Apellidos = GetRequired(row, "Apellidos"),
            FechaNacimiento = GetDate(row, "FechaNacimiento"),
            Direccion = GetRequired(row, "Direccion"),
            Telefono = GetRequired(row, "Telefono"),
            Email = GetRequired(row, "Email"),
            Cargo = cargo,
            Salario = GetDecimal(row, "Salario"),
            FechaIngreso = GetDate(row, "FechaIngreso"),
            Estado = estado,
            NivelEducativo = nivel,
            PerfilProfesional = GetRequired(row, "PerfilProfesional"),
            Departamento = departamento
        };
    }
    
    public void UpdateFromExcel(
        Dictionary<string, string> row)
    {
        var cargo = ParseCargo(GetRequired(row, "Cargo"));
        var estado = ParseEstado(GetRequired(row, "Estado"));
        var nivel = ParseNivelEducativo(GetRequired(row, "NivelEducativo"));
        var departamento = ParseDepartamento(GetRequired(row, "Departamento"));

        ValidateEnums(cargo, estado, nivel, departamento);

        Nombres = GetRequired(row, "Nombres");
        Apellidos = GetRequired(row, "Apellidos");
        FechaNacimiento = GetDate(row, "FechaNacimiento");
        Direccion = GetRequired(row, "Direccion");
        Telefono = GetRequired(row, "Telefono");
        Email = GetRequired(row, "Email");
        Cargo = cargo;
        Salario = GetDecimal(row, "Salario");
        FechaIngreso = GetDate(row, "FechaIngreso");
        Estado = estado;
        NivelEducativo = nivel;
        PerfilProfesional = GetRequired(row, "PerfilProfesional");
        Departamento = departamento;
    }

    private static void ValidateEnums(
        Cargo cargo,
        EstadoEmpleado estado,
        NivelEducativo nivel,
        Departamento departamento)
    {
        if (cargo == Cargo.Unknown)
            throw new InvalidOperationException("Invalid charge.");

        if (estado == EstadoEmpleado.Unknown)
            throw new InvalidOperationException("Invalid state.");

        if (nivel == NivelEducativo.Unknown)
                throw new InvalidOperationException("Invalid educational level.");

        if (departamento == Departamento.Unknown)
            throw new InvalidOperationException("Invalid department.");
    }

    
}
using Domain.Enums;

namespace Application.DTOs.Excel;

public class EmployeeExcelRowDto
{
    public string Documento { get; set; } = default!;
    public string Nombres { get; set; } = default!;
    public string Apellidos { get; set; } = default!;
    public string FechaNacimiento { get; set; } = default!;
    public string Direccion { get; set; } = default!;
    public string Telefono { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string Cargo { get; set; } = default!;
    public string Salario { get; set; } = default!;
    public string FechaIngreso { get; set; } = default!;
    public string Estado { get; set; } = default!;
    public string NivelEducativo { get; set; } = default!;
    public string PerfilProfesional { get; set; } = default!;
    public string Departamento { get; set; } = default!;
}

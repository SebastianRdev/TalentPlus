using System.ComponentModel.DataAnnotations;
using Domain.Enums;

namespace WebAdmin.ViewModels.Employees;

public class EmployeeUpdateViewModel
{
    public Guid Id { get; set; }

    [Required]
    [Display(Name = "Document Number")]
    public string Documento { get; set; } = default!;

    [Required]
    [Display(Name = "First Name")]
    public string Nombres { get; set; } = default!;

    [Required]
    [Display(Name = "Last Name")]
    public string Apellidos { get; set; } = default!;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = default!;

    [Required]
    public Cargo Cargo { get; set; }

    [Required]
    public Departamento Departamento { get; set; }

    [Required]
    public EstadoEmpleado Estado { get; set; }

    [Display(Name = "Hire Date")]
    public DateTime FechaIngreso { get; set; }
    
    public decimal Salario { get; set; }
    
    [Display(Name = "Date of Birth")]
    public DateTime FechaNacimiento { get; set; }
    
    public string? Direccion { get; set; }
    public string? Telefono { get; set; }
    public NivelEducativo NivelEducativo { get; set; }
    public string? PerfilProfesional { get; set; }
}

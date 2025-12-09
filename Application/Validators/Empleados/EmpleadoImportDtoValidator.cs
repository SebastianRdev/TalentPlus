namespace Application.Validators.Empleados;

using FluentValidation;
using Application.DTOs.Empleados;

public class EmployeeImportDtoValidator : AbstractValidator<EmployeeImportDto>
{
    public EmployeeImportDtoValidator()
    {
        RuleFor(x => x.Documento).NotEmpty();
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Nombres).NotEmpty();
        RuleFor(x => x.Apellidos).NotEmpty();
    }
}

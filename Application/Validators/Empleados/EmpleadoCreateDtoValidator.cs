using FluentValidation;
using Application.DTOs.Empleados;

namespace Application.Validators.Empleados;

public class EmployeeCreateDtoValidator : AbstractValidator<EmployeeCreateDto>
{
    public EmployeeCreateDtoValidator()
    {
        RuleFor(x => x.Documento)
            .NotEmpty().MaximumLength(50);

        RuleFor(x => x.Nombres)
            .NotEmpty().MaximumLength(100);

        RuleFor(x => x.Apellidos)
            .NotEmpty().MaximumLength(100);

        RuleFor(x => x.Email)
            .NotEmpty().EmailAddress();

        RuleFor(x => x.Salario)
            .GreaterThanOrEqualTo(0)
            .When(x => x.Salario.HasValue);

        RuleFor(x => x.FechaIngreso)
            .LessThanOrEqualTo(DateTime.UtcNow)
            .When(x => x.FechaIngreso.HasValue);
    }
}
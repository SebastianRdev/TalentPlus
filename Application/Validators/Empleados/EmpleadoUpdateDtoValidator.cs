using FluentValidation;
using Application.DTOs.Empleados;

namespace Application.Validators.Empleados;

public class EmployeeUpdateDtoValidator : AbstractValidator<EmployeeUpdateDto>
{
    public EmployeeUpdateDtoValidator()
    {
        RuleFor(x => x.Nombres).NotEmpty();
        RuleFor(x => x.Apellidos).NotEmpty();
        RuleFor(x => x.Email).NotEmpty().EmailAddress();

        RuleFor(x => x.Salario)
            .GreaterThanOrEqualTo(0)
            .When(x => x.Salario.HasValue);
    }
}

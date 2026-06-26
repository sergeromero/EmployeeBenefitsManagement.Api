using FluentValidation;

namespace Benefits.Application.Features.Employees.UpdateEmployee
{
    public class UpdateEmployeeValidator : AbstractValidator<UpdateEmployeeCommand>
    {
        public UpdateEmployeeValidator()
        {
            RuleFor(x => x.EmployeeNumber.ToString())
                .NotEmpty()
                .MinimumLength(8)
                .MaximumLength(8);

            RuleFor(x => x.FirstName)
                .NotEmpty()
                .MaximumLength(100);

            RuleFor(x => x.LastName)
                .NotEmpty()
                .MaximumLength(100);

            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress();

            RuleFor(x => x.DepartmentId)
                .GreaterThan(0);

            RuleFor(x => x.HireDate)
                .NotNull();
        }
    }
}

using FluentValidation;

namespace Benefits.Application.Features.Employees.CreateEmployee
{
    public class CreateEmployeeValidator : AbstractValidator<CreateEmployeeCommand>
    {
        public CreateEmployeeValidator()
        {
            RuleFor(x => x.EmployeeNumber)
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

            RuleFor(x => x.HireDate)
                .NotEmpty();

            RuleFor(x => x.DepartmentId)
                .GreaterThan(0);
        }
    }
}

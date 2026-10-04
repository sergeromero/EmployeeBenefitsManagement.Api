using FluentValidation;

namespace Benefits.Application.Features.Employees.UpdateEmployeeWithUser
{
    public sealed class UpdateEmployeeWithUserValidator :AbstractValidator<UpdateEmployeeWithUserCommand>
    {
        public UpdateEmployeeWithUserValidator()
        {
            RuleFor(x => x.Employee.Id).GreaterThan(0);

            RuleFor(x => x.Employee.FirstName).NotEmpty();
            RuleFor(x => x.Employee.LastName).NotEmpty();

            RuleFor(x => x.Employee.Email)
                .NotEmpty()
                .EmailAddress();

            RuleFor(x => x.Employee.DepartmentId)
                .GreaterThan(0);

            RuleFor(x => x.User).NotNull();

            RuleFor(x => x.User.UserName)
                .NotEmpty();

            RuleFor(x => x.User.Email)
                .EmailAddress();

            RuleFor(x => x.User.Password)
                .MinimumLength(6)
                .When(x => !string.IsNullOrWhiteSpace(x.User.Password));

            RuleFor(x => x.Role)
                .NotEmpty();
        }
    }
}

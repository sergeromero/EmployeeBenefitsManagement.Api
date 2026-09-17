using FluentValidation;

namespace Benefits.Application.Features.Employees.Queries.ViewEmployeeBenefits
{
    internal class GetEmployeeBenefitsValidator : AbstractValidator<GetEmployeeBenefitsQuery>
    {
        public GetEmployeeBenefitsValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0);
        }
    }
}

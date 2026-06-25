using FluentValidation;

namespace Benefits.Application.Features.Employees.Queries.SearchEmployees
{
    public class SearchEmployeesValidator : AbstractValidator<SearchEmployeesQuery>
    {
        public SearchEmployeesValidator()
        {
            RuleFor(x => x)
                .Must(HaveAtLeastOneFilter)
                .WithMessage("At least one search criteria must be provided.");
        }

        private bool HaveAtLeastOneFilter(SearchEmployeesQuery query)
        {
            return !string.IsNullOrWhiteSpace(query.Name)
                || (query.EmployeeNumber.HasValue)
                || query.DepartmentId.HasValue;
        }
    }
}

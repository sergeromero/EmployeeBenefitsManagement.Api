using MediatR;

namespace Benefits.Application.Features.Employees.Queries.ViewEmployeeBenefits
{
    public sealed record GetEmployeeBenefitsQuery(int Id) : IRequest<EmployeeBenefitsDto>;
}

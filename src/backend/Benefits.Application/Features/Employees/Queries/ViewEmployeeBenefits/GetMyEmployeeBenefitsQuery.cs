using MediatR;

namespace Benefits.Application.Features.Employees.Queries.ViewEmployeeBenefits
{
    public sealed record GetMyEmployeeBenefitsQuery : IRequest<IReadOnlyList<EmployeeBenefitDto>>;
}

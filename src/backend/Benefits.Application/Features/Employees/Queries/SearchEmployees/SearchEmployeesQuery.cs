using Benefits.Application.Features.Employees.Common;
using MediatR;

namespace Benefits.Application.Features.Employees.Queries.SearchEmployees
{
    public sealed record SearchEmployeesQuery(
        string? Name,
        string? EmployeeNumber,
        int? DepartmentId,
        int Page = 1,
        int PageSize = 10)
        : IRequest<List<EmployeeBasicInfoDto>>
    {
    }
}

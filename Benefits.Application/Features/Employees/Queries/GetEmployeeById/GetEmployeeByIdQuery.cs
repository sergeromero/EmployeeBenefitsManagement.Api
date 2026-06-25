using Benefits.Application.Features.Employees.Common;
using MediatR;

namespace Benefits.Application.Features.Employees.Queries.GetEmployeeById
{
    public sealed record GetEmployeeByIdQuery(int Id) : IRequest<EmployeeBasicInfoDto?>;
}

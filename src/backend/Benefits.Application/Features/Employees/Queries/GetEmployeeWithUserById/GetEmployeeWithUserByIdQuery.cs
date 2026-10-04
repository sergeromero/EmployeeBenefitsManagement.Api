using MediatR;

namespace Benefits.Application.Features.Employees.Queries.GetEmployeeWithUserById
{
    public sealed record GetEmployeeWithUserByIdQuery(int Id) : IRequest<EmployeeWithUserDto>;
}

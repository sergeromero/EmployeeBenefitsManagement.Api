using MediatR;

namespace Benefits.Application.Features.Employees.Queries.GetEmployeeById
{
    public sealed record GetEmployeeByIdQuery(int Id) : IRequest<GetEmployeeByIdResponse?>;
}

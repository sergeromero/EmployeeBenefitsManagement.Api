using MediatR;

namespace Benefits.Application.Features.Employees.DeleteEmployee
{
    public sealed record DeleteEmployeeCommand(int Id) : IRequest
    {
    }
}

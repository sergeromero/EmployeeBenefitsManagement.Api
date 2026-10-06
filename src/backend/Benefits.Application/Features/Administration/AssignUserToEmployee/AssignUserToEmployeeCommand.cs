using MediatR;

namespace Benefits.Application.Features.Administration.AssignUserToEmployee
{
    public sealed record AssignUserToEmployeeCommand(string UserId, int EmployeeId) : IRequest<Unit>;
}

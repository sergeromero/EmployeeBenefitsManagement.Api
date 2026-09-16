using MediatR;

namespace Benefits.Application.Features.Administration.AssignRoleToUser
{
    public record AssignRoleToUserCommand(string UserId, string Role) : IRequest<Unit>
    {
    }
}

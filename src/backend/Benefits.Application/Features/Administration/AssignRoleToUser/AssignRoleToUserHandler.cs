using Benefits.Application.Infrastructure.Contracts;
using Benefits.Common;
using MediatR;

namespace Benefits.Application.Features.Administration.AssignRoleToUser
{
    public class AssignRoleToUserHandler : IRequestHandler<AssignRoleToUserCommand, Unit>
    {
        private readonly IIdentityService _identityService;

        public AssignRoleToUserHandler(IIdentityService identityService)
        {
            _identityService = Guard.NotNull(identityService);
        }

        public async Task<Unit> Handle(AssignRoleToUserCommand request, CancellationToken cancellationToken)
        {
            await _identityService.AssignRoleToUserAsync(request.UserId, request.Role);
            return Unit.Value;
        }
    }
}

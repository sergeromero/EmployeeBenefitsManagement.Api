using Benefits.Application.Admin;
using Benefits.Application.Infrastructure.Contracts;
using Benefits.Common;
using MediatR;

namespace Benefits.Application.Features.BenefitPlans.Queries.GetUserRoles
{
    public sealed class GetUserRoleHandler : IRequestHandler<GetUserRolesQuery, IReadOnlyList<RoleDto>>
    {
        private readonly IIdentityService _identityService;

        public GetUserRoleHandler(IIdentityService identityService)
        {
            _identityService = Guard.NotNull(identityService);
        }

        public async Task<IReadOnlyList<RoleDto>> Handle(GetUserRolesQuery request, CancellationToken cancellationToken)
        {
            return await _identityService.GetRolesAsync();
        }
    }
}

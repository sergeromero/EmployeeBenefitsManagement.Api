using Benefits.Application.Admin;
using MediatR;

namespace Benefits.Application.Features.BenefitPlans.Queries.GetUserRoles
{
    public sealed record GetUserRolesQuery : IRequest<IReadOnlyList<RoleDto>>;
}

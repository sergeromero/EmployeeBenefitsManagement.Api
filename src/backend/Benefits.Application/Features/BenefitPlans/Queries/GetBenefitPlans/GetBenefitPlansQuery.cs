using MediatR;

namespace Benefits.Application.Features.BenefitPlans.Queries.GetBenefitPlans
{
    public sealed record GetBenefitPlansQuery : IRequest<IReadOnlyList<BenefitPlanListItemDto>>;
}

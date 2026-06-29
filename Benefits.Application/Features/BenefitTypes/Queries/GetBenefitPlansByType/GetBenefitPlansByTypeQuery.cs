using MediatR;

namespace Benefits.Application.Features.BenefitTypes.Queries.GetBenefitPlansByType
{
    public sealed record GetBenefitPlansByTypeQuery(int Id) : IRequest<IReadOnlyList<BenefitPlanListItemDto>>;
}

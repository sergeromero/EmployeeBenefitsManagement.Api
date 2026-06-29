namespace Benefits.Application.Features.BenefitPlans.Queries.GetBenefitPlans
{
    public sealed record BenefitPlanListItemDto(
    int Id,
    string Name,
    BenefitTypeDto BenefitType);
}

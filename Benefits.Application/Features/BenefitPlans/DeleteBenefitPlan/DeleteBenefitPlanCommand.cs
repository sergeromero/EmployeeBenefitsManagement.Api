using MediatR;

namespace Benefits.Application.Features.BenefitPlans.DeleteBenefitPlan
{
    public sealed record DeleteBenefitPlanCommand(int Id) : IRequest;
}

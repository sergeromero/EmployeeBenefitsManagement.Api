using MediatR;

namespace Benefits.Application.Features.BenefitPlans.CreateBenefitPlans
{
    public sealed record CreateBenefitPlansCommand(
        string Name,
        string Description,
        int BenefitTypeId) : IRequest<int>;
}

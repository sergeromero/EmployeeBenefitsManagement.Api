using MediatR;

namespace Benefits.Application.Features.BenefitTypes.CreateBenefitTypes
{
    public sealed record CreateBenefitTypesCommand(string Name) : IRequest<int>;
}

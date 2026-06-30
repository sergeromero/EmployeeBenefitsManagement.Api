using FluentValidation;

namespace Benefits.Application.Features.BenefitPlans.DeleteBenefitPlan
{
    internal sealed class DeleteBenefitPlanValidator : AbstractValidator<DeleteBenefitPlanCommand>
    {
        public DeleteBenefitPlanValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0);
        }
    }
}

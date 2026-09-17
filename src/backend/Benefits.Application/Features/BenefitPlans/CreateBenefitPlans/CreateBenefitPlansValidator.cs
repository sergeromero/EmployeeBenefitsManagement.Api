using FluentValidation;

namespace Benefits.Application.Features.BenefitPlans.CreateBenefitPlans
{
    internal class CreateBenefitPlansValidator : AbstractValidator<CreateBenefitPlansCommand>
    {
        public CreateBenefitPlansValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .MaximumLength(80);

            RuleFor(x => x.Description)
                .NotEmpty()
                .MaximumLength(300);

            RuleFor(x => x.BenefitTypeId)
                .GreaterThan(0);
        }
    }
}

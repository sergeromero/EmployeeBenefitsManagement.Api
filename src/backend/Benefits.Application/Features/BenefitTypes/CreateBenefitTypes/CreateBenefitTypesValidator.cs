using FluentValidation;

namespace Benefits.Application.Features.BenefitTypes.CreateBenefitTypes
{
    internal class CreateBenefitTypesValidator : AbstractValidator<CreateBenefitTypesCommand>
    {
        public CreateBenefitTypesValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .MaximumLength(50);
        }
    }
}

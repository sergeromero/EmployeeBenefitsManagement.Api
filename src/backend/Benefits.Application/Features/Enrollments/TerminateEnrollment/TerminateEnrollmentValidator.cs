using FluentValidation;

namespace Benefits.Application.Features.Enrollments.TerminateEnrollment
{
    public sealed class TerminateEnrollmentValidator : AbstractValidator<TerminateEnrollmentCommand>
    {
        public TerminateEnrollmentValidator()
        {
            RuleFor(x => x.EmployeeId)
                .GreaterThan(0);

            RuleFor(x => x.BenefitPlanId)
                .GreaterThan(0);

            RuleFor(x => x.EndDate)
                .Must(x => x != default)
                .NotEmpty();
        }
    }
}

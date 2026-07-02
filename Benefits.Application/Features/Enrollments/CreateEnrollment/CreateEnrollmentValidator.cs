using FluentValidation;

namespace Benefits.Application.Features.Enrollments.CreateEnrollment
{
    internal class CreateEnrollmentValidator : AbstractValidator<CreateEnrollmentCommand>
    {
        public CreateEnrollmentValidator()
        {
            RuleFor(x => x.EmployeeId)
                .GreaterThan(0);

            RuleFor(x => x.BenefitPlanId)
                .GreaterThan(0);

            RuleFor(x => x.EnrollmentDate)
                .Must(x => x != default)
                .NotEmpty();
        }
    }
}

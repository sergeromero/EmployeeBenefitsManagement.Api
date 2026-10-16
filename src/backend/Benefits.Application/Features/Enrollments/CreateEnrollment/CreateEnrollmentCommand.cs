using MediatR;

namespace Benefits.Application.Features.Enrollments.CreateEnrollment
{
    public sealed record CreateEnrollmentCommand(
        int BenefitPlanId,
        DateOnly EnrollmentDate) : IRequest<Unit>;
}

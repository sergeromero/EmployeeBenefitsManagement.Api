using MediatR;

namespace Benefits.Application.Features.Enrollments.TerminateEnrollment
{
    public sealed record TerminateEnrollmentCommand(int BenefitPlanId, DateOnly EndDate) : IRequest;
}

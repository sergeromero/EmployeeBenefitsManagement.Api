using MediatR;

namespace Benefits.Application.Features.Enrollments.CreateEnrollment
{
    public sealed record CreateEnrollmentCommand(
        int EmployeeId,
        int BenefitPlanId,
        DateOnly EnrollmentDate) : IRequest<Unit>;
}

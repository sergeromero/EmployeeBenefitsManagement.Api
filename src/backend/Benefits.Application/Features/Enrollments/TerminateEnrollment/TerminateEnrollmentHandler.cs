using Benefits.Application.Exceptions;
using Benefits.Application.Exceptions.BusinessRuleViolationException;
using Benefits.Application.Infrastructure.Contracts;
using Benefits.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Benefits.Application.Features.Enrollments.TerminateEnrollment
{
    public sealed class TerminateEnrollmentHandler : IRequestHandler<TerminateEnrollmentCommand>
    {
        private readonly IBenefitsDbContext _dbContext;

        public TerminateEnrollmentHandler(IBenefitsDbContext dbContext)
        {
            _dbContext = Guard.NotNull(dbContext);
        }

        public async Task Handle(TerminateEnrollmentCommand command, CancellationToken cancellationToken)
        {
            var enrollment = await _dbContext.EmployeeEnrollments.Where(ee => ee.EmployeeId == command.EmployeeId && ee.BenefitPlanId == command.BenefitPlanId)
                .SingleOrDefaultAsync(cancellationToken)
                ?? throw new NotFoundException($"No enrollment found with Employee Id {command.EmployeeId} and Benefit Plan Id {command.BenefitPlanId}.");

            if (!enrollment.IsActive)
            {
                throw new EnrollmentAlreadyTerminatedException($"Enrollment with Employee Id {command.EmployeeId} and Benefit Plan Id {command.BenefitPlanId} is already terminated.");
            }

            if (enrollment.EnrollmentDate > command.EndDate)
            {
                throw new InvalidEnrollmentDateException("An enrollment cannot end before it started.");
            }

            enrollment.EndEnrollment(command.EndDate);

            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}

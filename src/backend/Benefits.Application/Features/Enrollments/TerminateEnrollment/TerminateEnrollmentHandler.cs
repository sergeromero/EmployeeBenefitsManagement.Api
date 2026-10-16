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
        private readonly ICurrentUserService _currentUserService;

        public TerminateEnrollmentHandler(IBenefitsDbContext dbContext, ICurrentUserService currentUserService)
        {
            _dbContext = Guard.NotNull(dbContext);
            _currentUserService = Guard.NotNull(currentUserService);
        }

        public async Task Handle(TerminateEnrollmentCommand command, CancellationToken cancellationToken)
        {
            var employeeId = await GetCurrentEmployeeId(cancellationToken);

            var enrollment = await _dbContext.EmployeeEnrollments.SingleOrDefaultAsync(ee => ee.EmployeeId == employeeId && ee.BenefitPlanId == command.BenefitPlanId, cancellationToken)
                ?? throw new NotFoundException($"No enrollment found with Employee Id {employeeId} and Benefit Plan Id {command.BenefitPlanId}.");

            if (!enrollment.IsActive)
            {
                throw new EnrollmentAlreadyTerminatedException($"Enrollment with Employee Id {employeeId} and Benefit Plan Id {command.BenefitPlanId} is already terminated.");
            }

            if (enrollment.EnrollmentDate > command.EndDate)
            {
                throw new InvalidEnrollmentDateException("An enrollment cannot end before it started.");
            }

            enrollment.EndEnrollment(command.EndDate);

            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        private async Task<int> GetCurrentEmployeeId(CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;

            if (string.IsNullOrWhiteSpace(userId))
            {
                throw new NotFoundException("No active employee record is linked to the authenticated user.");
            }

            var employeeId = await _dbContext.Employees
                .Where(e => e.UserId == userId && e.IsActive)
                .Select(e => (int?)e.Id)
                .SingleOrDefaultAsync(cancellationToken);

            return employeeId ?? throw new NotFoundException("No active employee record is linked to the authenticated user.");
        }
    }
}

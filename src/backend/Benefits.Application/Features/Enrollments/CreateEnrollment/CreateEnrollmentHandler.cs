using Benefits.Application.Exceptions;
using Benefits.Application.Exceptions.BusinessRuleViolationException;
using Benefits.Application.Infrastructure.Contracts;
using Benefits.Common;
using Benefits.Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Benefits.Application.Features.Enrollments.CreateEnrollment
{
    public class CreateEnrollmentHandler : IRequestHandler<CreateEnrollmentCommand, Unit>
    {
        private readonly IBenefitsDbContext _dbContext;
        private readonly ICurrentUserService _currentUserService;
        private readonly TimeProvider _timeProvider;

        public CreateEnrollmentHandler(IBenefitsDbContext dbContext, ICurrentUserService currentUserService, TimeProvider timeProvider)
        {
            _dbContext = Guard.NotNull(dbContext);
            _timeProvider = Guard.NotNull(timeProvider);
            _currentUserService = Guard.NotNull(currentUserService);
        }

        public async Task<Unit> Handle(CreateEnrollmentCommand request, CancellationToken cancellationToken)
        {
            var employeeId = await GetCurrentEmployeeId(cancellationToken);

            await ValidateRequest(request, employeeId, cancellationToken);

            var newEnrollment = new EmployeeEnrollment
            {
                EmployeeId = employeeId,
                BenefitPlanId = request.BenefitPlanId,
                EnrollmentDate = request.EnrollmentDate
            };

            _dbContext.EmployeeEnrollments.Add(newEnrollment);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return Unit.Value;
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

        private async Task ValidateRequest(CreateEnrollmentCommand request, int employeeId, CancellationToken cancellationToken)
        {
            var benefitPlan = await _dbContext.BenefitPlans.Where(bp => bp.Id == request.BenefitPlanId && bp.IsActive)
                .Select(bp => new
                {
                    bp.Id,
                    bp.EnrollmentCategoryId,
                    bp.Name
                })
                .SingleOrDefaultAsync(cancellationToken) 
                ?? throw new NotFoundException($"Benefit Plan with id {request.BenefitPlanId} was not found.");

            await EnsureNonDuplicateEnrollment(employeeId, benefitPlan.EnrollmentCategoryId, benefitPlan.Name, cancellationToken);

            EnsureValidDate(request.EnrollmentDate);
        }

        private async Task EnsureNonDuplicateEnrollment(int employeeId, int enrollmentCategoryId, string benefitPlanName, CancellationToken cancellationToken)
        {
            var today = DateOnly.FromDateTime(_timeProvider.GetUtcNow().DateTime);

            var isAlreadyEnrolled = await _dbContext.EmployeeEnrollments.AnyAsync(ee => ee.EmployeeId == employeeId
                        && ee.BenefitPlan.EnrollmentCategoryId == enrollmentCategoryId
                        && (ee.EndDate == null || ee.EndDate >= today), cancellationToken);

            if (isAlreadyEnrolled)
            {
                throw new BusinessRuleException($"You cannot be enrolled in benefit plan {benefitPlanName} because you already have an enrollment in the same enrollment category.");
            }
        }

        private void EnsureValidDate(DateOnly enrollmentDate)
        {
            var today = DateOnly.FromDateTime(_timeProvider.GetUtcNow().DateTime);
            if (enrollmentDate < today)
            {
                throw new BusinessRuleException("Enrollment date cannot be in the past.");
            }
        }
    }
}

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
        private sealed record Test();

        private readonly IBenefitsDbContext _dbContext;
        private readonly TimeProvider _timeProvider;

        public CreateEnrollmentHandler(IBenefitsDbContext dbContext, TimeProvider timeProvider)
        {
            _dbContext = Guard.NotNull(dbContext);
            _timeProvider = Guard.NotNull(timeProvider);
        }

        public async Task<Unit> Handle(CreateEnrollmentCommand request, CancellationToken cancellationToken)
        {
            await ValidateRequest(request, cancellationToken);

            var newEnrollment = new EmployeeEnrollment
            {
                EmployeeId = request.EmployeeId,
                BenefitPlanId = request.BenefitPlanId,
                EnrollmentDate = request.EnrollmentDate
            };

            _dbContext.EmployeeEnrollments.Add(newEnrollment);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }

        private async Task ValidateRequest(CreateEnrollmentCommand request, CancellationToken cancellationToken)
        {
            await EnsureEmployeeExists(request, cancellationToken);

            var benefitPlan = await _dbContext.BenefitPlans.Where(bp => bp.Id == request.BenefitPlanId && bp.IsActive)
                .Select(bp => new
                {
                    bp.Id,
                    bp.EnrollmentCategoryId,
                    bp.Name
                })
                .SingleOrDefaultAsync(cancellationToken) 
                ?? throw new NotFoundException($"Benefit Plan with id {request.BenefitPlanId} was not found");

            await EnsureNonDuplicateEnrollment(request, benefitPlan.EnrollmentCategoryId, benefitPlan.Name, cancellationToken);

            EnsureValidDate(request.EnrollmentDate);
        }

        private async Task EnsureEmployeeExists(CreateEnrollmentCommand request, CancellationToken cancellationToken)
        {
            var employeeExists = await _dbContext.Employees.AnyAsync(e => e.Id == request.EmployeeId && e.IsActive, cancellationToken);
            if (!employeeExists)
            {
                throw new NotFoundException($"Employee with id {request.EmployeeId} was not found");
            }
        }

        private async Task EnsureNonDuplicateEnrollment(CreateEnrollmentCommand request, int enrollmentCategoryId, string benefitPlanName, CancellationToken cancellationToken)
        {
            var isAlreadyEnrolled = await _dbContext.EmployeeEnrollments.AnyAsync(ee => ee.EmployeeId == request.EmployeeId
                        && ee.BenefitPlan.EnrollmentCategoryId == enrollmentCategoryId, cancellationToken);

            if (isAlreadyEnrolled)
            {
                throw new BusinessRuleException($"Employee cannot be enrolled in benefit plan {benefitPlanName} because they already have an enrollment in the same enrollment category.");
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

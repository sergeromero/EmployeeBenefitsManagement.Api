using Benefits.Application.Exceptions;
using Benefits.Application.Infrastructure.Contracts;
using Benefits.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Numerics;

namespace Benefits.Application.Features.BenefitPlans.Queries.GetBenefitPlans
{
    public sealed class GetBenefitPlansHandler : IRequestHandler<GetBenefitPlansQuery, IReadOnlyList<BenefitPlanListItemDto>>
    {
        private const string MissingEmployeeMessage = "No active employee record is linked to the authenticated user.";
        private readonly IBenefitsDbContext _dbContext;
        private readonly ICurrentUserService _currentUserService;

        public GetBenefitPlansHandler(IBenefitsDbContext dbContext, ICurrentUserService currentUserService)
        {
            _dbContext = Guard.NotNull(dbContext);
            _currentUserService = Guard.NotNull(currentUserService);
        }

        public async Task<IReadOnlyList<BenefitPlanListItemDto>> Handle(GetBenefitPlansQuery request, CancellationToken cancellationToken)
        {
            var employeeId = await GetCurrentEmployeeIdAsync(cancellationToken);

            var activeEnrollments = await _dbContext.EmployeeEnrollments
                .AsNoTracking()
                .Where(e => e.EmployeeId == employeeId && e.EndDate == null)
                .Select(e => new
                {
                    e.BenefitPlanId,
                    e.BenefitPlan.EnrollmentCategoryId
                }).ToListAsync(cancellationToken);

            var enrolledCategoryIds = activeEnrollments.Select(e => e.EnrollmentCategoryId).ToHashSet();

            var enrolledBenefitPlanIds = activeEnrollments.Select(e => e.BenefitPlanId).ToHashSet();

            var benefitPlans = await _dbContext.BenefitPlans
                .AsNoTracking()
                .Where(p => p.IsActive)
                .OrderBy(p => p.BenefitType.Name)
                .ThenBy(p => p.Name)
                .Select(p => new
                {
                    p.Id,
                    p.Name,
                    p.Description,
                    BenefitType = new BenefitTypeDto(
                        p.BenefitType.Id,
                        p.BenefitType.Name),
                    p.EnrollmentCategoryId,
                    EnrollmentCategoryName = p.EnrollmentCategory.Name
                }).ToListAsync(cancellationToken);

            return benefitPlans.Select(p =>
            {
                EnrollmentIneligibilityReason? reason = enrolledBenefitPlanIds.Contains(p.Id)
                    ? EnrollmentIneligibilityReason.AlreadyEnrolledInBenefit
                    : enrolledCategoryIds.Contains(p.EnrollmentCategoryId)
                        ? EnrollmentIneligibilityReason.AlreadyEnrolledInCategory
                        : null;

                return new BenefitPlanListItemDto(
                    p.Id,
                    p.Name,
                    p.Description,
                    p.BenefitType,
                    p.EnrollmentCategoryName,
                    new EnrollmentEligibilityDto(
                        CanEnroll: reason is null,
                        Reason: reason));
            }).ToList();
        }

        private async Task<int> GetCurrentEmployeeIdAsync(CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;

            if (string.IsNullOrWhiteSpace(userId))
            {
                throw new NotFoundException(MissingEmployeeMessage);
            }

            var employeeId = await _dbContext.Employees
                .AsNoTracking()
                .Where(employee => employee.UserId == userId && employee.IsActive)
                .Select(employee => (int?)employee.Id)
                .SingleOrDefaultAsync(cancellationToken);

            return employeeId ?? throw new NotFoundException(MissingEmployeeMessage);
        }
    }
}

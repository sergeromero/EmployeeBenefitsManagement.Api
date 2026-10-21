using Benefits.Application.Exceptions;
using Benefits.Application.Infrastructure.Contracts;
using Benefits.Common;
using Benefits.Domain;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Benefits.Application.Features.Employees.Queries.ViewEmployeeBenefits
{
    public abstract class GetEmployeeBenefitsHandlerBase
    {
        protected readonly IBenefitsDbContext _dbContext;
        protected readonly TimeProvider _timeProvider;

        protected GetEmployeeBenefitsHandlerBase(IBenefitsDbContext dbContext, TimeProvider timeProvider)
        {
            _dbContext = Guard.NotNull(dbContext);
            _timeProvider = Guard.NotNull(timeProvider);
        }

        private static Expression<Func<EmployeeEnrollment, EmployeeBenefitDto>> MapBenefitDto(DateOnly today) =>
            ee => new EmployeeBenefitDto(
                ee.BenefitPlanId,
                ee.BenefitPlan.Name,
                ee.BenefitPlan.BenefitType.Name,
                ee.EnrollmentDate,
                ee.EndDate,
                ee.EndDate == null || ee.EndDate >= today
            );

        protected async Task<EmployeeBenefitsDto> GetEmployeeBenefitsAsync(IQueryable<Employee> employeeQuery, string notFoundMessage, CancellationToken cancellationToken)
        {
            var today = DateOnly.FromDateTime(_timeProvider.GetUtcNow().DateTime);

            return await employeeQuery.Select(e => new EmployeeBenefitsDto(
                e.Id,
                e.FirstName,
                e.LastName,
                e.Department.Name,
                e.Enrollments
                    .OrderBy(ee => ee.BenefitPlan.Name)
                    .AsQueryable()
                    .Select(MapBenefitDto(today))
                    .ToList()
            )).FirstOrDefaultAsync(cancellationToken) ?? throw new NotFoundException(notFoundMessage);
        }

        protected async Task<IReadOnlyList<EmployeeBenefitDto>> GetEmployeeBenefitListAsync(IQueryable<Employee> employeeQuery, string notFoundMessage, CancellationToken cancellationToken)
        {
            var today = DateOnly.FromDateTime(_timeProvider.GetUtcNow().DateTime);

            var benefits = await employeeQuery
                .Select(e => e.Enrollments
                    .OrderBy(ee => ee.BenefitPlan.Name)
                    .AsQueryable()
                    .Select(MapBenefitDto(today))
                    .ToList())
                .FirstOrDefaultAsync(cancellationToken);

            return benefits ?? throw new NotFoundException(notFoundMessage);
        }
    }
}

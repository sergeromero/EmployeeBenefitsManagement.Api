using Benefits.Application.Exceptions;
using Benefits.Application.Infrastructure.Contracts;
using Benefits.Common;
using Benefits.Domain;
using Microsoft.EntityFrameworkCore;

namespace Benefits.Application.Features.Employees.Queries.ViewEmployeeBenefits
{
    public abstract class GetEmployeeBenefitsHandlerBase
    {
        protected readonly IBenefitsDbContext _dbContext;

        protected GetEmployeeBenefitsHandlerBase(IBenefitsDbContext dbContext)
        {
            _dbContext = Guard.NotNull(dbContext);
        }

        protected async Task<EmployeeBenefitsDto> GetEmployeeBenefitsAsync(IQueryable<Employee> employeeQuery, string notFoundMessage, CancellationToken cancellationToken)
        {
            return await employeeQuery.Select(e => new EmployeeBenefitsDto
                (
                    e.Id,
                    e.FirstName,
                    e.LastName,
                    e.Department.Name,
                    e.Enrollments
                    .OrderBy(ee => ee.BenefitPlan.Name)
                    .Select(ee => new EmployeeBenefitDto
                    (
                        ee.BenefitPlanId,
                        ee.BenefitPlan.Name,
                        ee.BenefitPlan.BenefitType.Name,
                        ee.EnrollmentDate,
                        ee.EndDate,
                        ee.EndDate == null
                    ))
                    .ToList()
                )).FirstOrDefaultAsync(cancellationToken)
                ?? throw new NotFoundException(notFoundMessage);
        }
    }
}

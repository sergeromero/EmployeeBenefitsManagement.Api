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

        protected GetEmployeeBenefitsHandlerBase(IBenefitsDbContext dbContext)
        {
            _dbContext = Guard.NotNull(dbContext);
        }

        private static readonly Expression<Func<EmployeeEnrollment, EmployeeBenefitDto>> MapBenefitDto =
        ee => new EmployeeBenefitDto(
            ee.BenefitPlanId,
            ee.BenefitPlan.Name,
            ee.BenefitPlan.BenefitType.Name,
            ee.EnrollmentDate,
            ee.EndDate,
            ee.EndDate == null
        );

        protected async Task<EmployeeBenefitsDto> GetEmployeeBenefitsAsync(IQueryable<Employee> employeeQuery, string notFoundMessage, CancellationToken cancellationToken)
        {
            return await employeeQuery.Select(e => new EmployeeBenefitsDto(
                e.Id,
                e.FirstName,
                e.LastName,
                e.Department.Name,
                e.Enrollments
                    .OrderBy(ee => ee.BenefitPlan.Name)
                    .AsQueryable()
                    .Select(MapBenefitDto)
                    .ToList()
            )).FirstOrDefaultAsync(cancellationToken) ?? throw new NotFoundException(notFoundMessage);
        }

        protected async Task<IReadOnlyList<EmployeeBenefitDto>> GetEmployeeBenefitListAsync(IQueryable<Employee> employeeQuery, string notFoundMessage, CancellationToken cancellationToken)
        {
            var benefits = await employeeQuery
                .Select(e => e.Enrollments
                    .OrderBy(ee => ee.BenefitPlan.Name)
                    .AsQueryable()
                    .Select(MapBenefitDto)
                    .ToList())
                .FirstOrDefaultAsync(cancellationToken);

            return benefits ?? throw new NotFoundException(notFoundMessage);
        }

        //protected async Task<EmployeeBenefitsDto> GetEmployeeBenefitsAsync(IQueryable<Employee> employeeQuery, string notFoundMessage, CancellationToken cancellationToken)
        //{
        //    return await employeeQuery.Select(e => new EmployeeBenefitsDto
        //        (
        //            e.Id,
        //            e.FirstName,
        //            e.LastName,
        //            e.Department.Name,
        //            e.Enrollments
        //            .OrderBy(ee => ee.BenefitPlan.Name)
        //            .Select(ee => new EmployeeBenefitDto
        //            (
        //                ee.BenefitPlanId,
        //                ee.BenefitPlan.Name,
        //                ee.BenefitPlan.BenefitType.Name,
        //                ee.EnrollmentDate,
        //                ee.EndDate,
        //                ee.EndDate == null
        //            ))
        //            .ToList()
        //        )).FirstOrDefaultAsync(cancellationToken)
        //        ?? throw new NotFoundException(notFoundMessage);
        //}

        //protected async Task<IReadOnlyList<EmployeeBenefitDto>> GetEmployeeBenefitListAsync(IQueryable<Employee> employeeQuery, string notFoundMessage, CancellationToken cancellationToken)
        //{
        //    var benefits = await employeeQuery
        //        .Select(e => e.Enrollments
        //            .OrderBy(enrollment => enrollment.BenefitPlan.Name)
        //            .Select(enrollment => new EmployeeBenefitDto(
        //                enrollment.BenefitPlanId,
        //                enrollment.BenefitPlan.Name,
        //                enrollment.BenefitPlan.BenefitType.Name,
        //                enrollment.EnrollmentDate,
        //                enrollment.EndDate,
        //                enrollment.EndDate == null))
        //            .ToList())
        //        .FirstOrDefaultAsync(cancellationToken);

        //    return benefits
        //        ?? throw new NotFoundException(notFoundMessage);
        //}
    }
}

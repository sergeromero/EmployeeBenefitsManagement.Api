using Benefits.Application.Exceptions;
using Benefits.Application.Infrastructure.Contracts;
using Benefits.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Benefits.Application.Features.Employees.Queries.ViewEmployeeBenefits
{
    public sealed class GetEmployeeBenefitsHandler : IRequestHandler<GetEmployeeBenefitsQuery, EmployeeBenefitsDto>
    {
        private readonly IBenefitsDbContext _dbContext;

        public GetEmployeeBenefitsHandler(IBenefitsDbContext dbContext)
        {
            _dbContext = Guard.NotNull(dbContext);
        }

        public async Task<EmployeeBenefitsDto> Handle(GetEmployeeBenefitsQuery request, CancellationToken cancellationToken)
        {
            var employeeBenefits = await _dbContext.Employees.Where(e => e.Id == request.Id && e.IsActive)
                .Select(e => new EmployeeBenefitsDto
                (
                    e.Id,
                    e.FirstName,
                    e.LastName,
                    e.Department.Name,
                    e.Enrollments
                    .OrderBy(ee => ee.BenefitPlan)
                    .Select(ee => new EmployeeBenefitDto
                    (
                        ee.BenefitPlanId,
                        ee.BenefitPlan.Name,
                        ee.BenefitPlan.BenefitType.Name
                    ))
                    .ToList()
                )).FirstOrDefaultAsync(cancellationToken)
                ?? throw new NotFoundException($"Employee with id {request.Id} was not found");

            return employeeBenefits;
        }
    }
}

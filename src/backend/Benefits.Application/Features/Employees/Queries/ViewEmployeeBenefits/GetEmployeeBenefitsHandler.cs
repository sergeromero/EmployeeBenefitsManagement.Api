using Benefits.Application.Infrastructure.Contracts;
using MediatR;

namespace Benefits.Application.Features.Employees.Queries.ViewEmployeeBenefits
{

    public sealed class GetEmployeeBenefitsHandler : GetEmployeeBenefitsHandlerBase, IRequestHandler<GetEmployeeBenefitsQuery, EmployeeBenefitsDto>
    {
        public GetEmployeeBenefitsHandler(IBenefitsDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<EmployeeBenefitsDto> Handle(GetEmployeeBenefitsQuery request, CancellationToken cancellationToken)
        {
            var employeeQuery = _dbContext.Employees.Where(e => e.Id == request.Id && e.IsActive);

            var employeeBenefits = await GetEmployeeBenefitsAsync(employeeQuery, $"Employee with id {request.Id} was not found.", cancellationToken);                

            return employeeBenefits;
        }
    }
}

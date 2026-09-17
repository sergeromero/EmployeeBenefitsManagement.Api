using Benefits.Application.Features.Employees.Common;
using Benefits.Application.Infrastructure.Contracts;
using Benefits.Common;
using Benefits.Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Benefits.Application.Features.Employees.Queries.SearchEmployees
{
    public class SearchEmployeesHandler : IRequestHandler<SearchEmployeesQuery, List<EmployeeBasicInfoDto>>
    {
        private readonly IBenefitsDbContext _dbContext;

        public SearchEmployeesHandler(IBenefitsDbContext context)
        {
            _dbContext = Guard.NotNull(context);
        }

        public async Task<List<EmployeeBasicInfoDto>> Handle(SearchEmployeesQuery request, CancellationToken cancellationToken)
        {
            IQueryable<Employee> query = _dbContext.Employees.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.Name))
            {
                var searchTerm = request.Name.Trim();

                query = query.Where(e =>
                    e.FirstName.Contains(searchTerm) ||
                    e.LastName.Contains(searchTerm));
            }

            if (!string.IsNullOrWhiteSpace(request.EmployeeNumber))
            {
                query = query.Where(e => e.EmployeeNumber == request.EmployeeNumber);
            }

            if (request.DepartmentId.HasValue)
            {
                query = query.Where(e => e.DepartmentId == request.DepartmentId);
            }

            return await query.Select(e => new EmployeeBasicInfoDto
            {
                Id = e.Id,
                FirstName = e.FirstName,
                LastName = e.LastName,
                HireDate = e.HireDate,
                Email = e.Email,
                EmployeeNumber = e.EmployeeNumber,
                DepartmentId = e.DepartmentId
            }).ToListAsync(cancellationToken);
        }
    }
}

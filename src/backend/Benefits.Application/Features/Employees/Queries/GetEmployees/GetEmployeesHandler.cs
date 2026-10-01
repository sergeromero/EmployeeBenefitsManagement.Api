using Benefits.Application.Common;
using Benefits.Application.Infrastructure.Contracts;
using Benefits.Common;
using Benefits.Domain.Constants;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Benefits.Application.Features.Employees.Queries.GetEmployees
{
    public sealed class GetEmployeesHandler : IRequestHandler<GetEmployeesQuery, PagedResult<EmployeeListItemDto>>
    {
        private readonly IBenefitsDbContext _dbContext;
        private readonly ICurrentUserService _userService;
        private readonly IIdentityService _identityService;

        public GetEmployeesHandler(ICurrentUserService userService, IBenefitsDbContext dbContext, IIdentityService identityService)
        {
            _userService = Guard.NotNull(userService);
            _dbContext = Guard.NotNull(dbContext);
            _identityService = Guard.NotNull(identityService);
        }

        public async Task<PagedResult<EmployeeListItemDto>> Handle(GetEmployeesQuery request, CancellationToken cancellationToken)
        {
            var query = _dbContext.Employees
                .Include(e => e.Department).AsQueryable();

            query = request.SortBy?.ToLower() switch
            {
                "employeenumber" => request.SortDirection == "desc"
                    ? query.OrderByDescending(e => e.EmployeeNumber)
                    : query.OrderBy(e => e.EmployeeNumber),
                "firstname" => request.SortDirection == "desc"
                    ? query.OrderByDescending(e => e.FirstName)
                    : query.OrderBy(e => e.FirstName),
                "lastname" => request.SortDirection == "desc"
                    ? query.OrderByDescending(e => e.LastName)
                    : query.OrderBy(e => e.LastName),
                "email" => request.SortDirection == "desc"
                    ? query.OrderByDescending(e => e.Email)
                    : query.OrderBy(e => e.Email),
                "hiredate" => request.SortDirection == "desc"
                    ? query.OrderByDescending(e => e.HireDate)
                    : query.OrderBy(e => e.HireDate),
                _ => query.OrderBy(e => e.LastName)
            };

            var totalCount = await query.CountAsync(cancellationToken);

            var employees = await query
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            var userIds = employees.Where(e => e.UserId != null).Select(e => e.UserId).ToList();

            var isAdmin = _userService.IsInRole(Roles.Administrator);

            Dictionary<string, string> userNames = [];

            if(isAdmin && userIds.Count > 0)
            {
                userNames = await _identityService.GetUserNamesByIdsAsync(userIds, cancellationToken);
            }

            var items = employees.Select(e => new EmployeeListItemDto
            {
                EmployeeNumber = e.EmployeeNumber,
                FirstName = e.FirstName,
                LastName = e.LastName,
                Email = e.Email,
                HireDate = e.HireDate,
                DepartmentName = e.Department.Name,
                UserName = isAdmin && e.UserId != null && userNames.ContainsKey(e.UserId) ? userNames[e.UserId] : null,
                HasUser = !isAdmin ? e.UserId != null : null
            }).ToList();

            return new PagedResult<EmployeeListItemDto>
            {
                Items = items,
                TotalCount = totalCount,
                Page = request.Page,
                PageSize = request.PageSize
            };
        }
    }
}

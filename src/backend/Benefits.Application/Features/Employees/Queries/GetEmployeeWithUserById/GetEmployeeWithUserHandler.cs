using Benefits.Application.Features.Employees.Common;
using Benefits.Application.Infrastructure.Contracts;
using Benefits.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Benefits.Application.Features.Employees.Queries.GetEmployeeWithUserById
{
    public sealed class GetEmployeeWithUserHandler : IRequestHandler<GetEmployeeWithUserByIdQuery, EmployeeWithUserDto>
    {
        private readonly IBenefitsDbContext _dbContext;
        private readonly IIdentityService _identityService;

        public GetEmployeeWithUserHandler(IBenefitsDbContext dbContext, IIdentityService identityService)
        {
            _dbContext = Guard.NotNull(dbContext);
            _identityService = Guard.NotNull(identityService);
        }
        public async Task<EmployeeWithUserDto> Handle(GetEmployeeWithUserByIdQuery request, CancellationToken cancellationToken)
        {
            var employee = await _dbContext.Employees.FirstAsync(e => e.Id == request.Id, cancellationToken);

            if(employee.UserId == null)
            {
                throw new InvalidOperationException("Selected Employee does not have a valid user assigned.");
            }

            var user = await _identityService.GetUserByIdAsync(employee.UserId);
            var role = await _identityService.GetUserRoleAsync(employee.UserId);

            var dto = new EmployeeWithUserDto
            {
                Employee = new EmployeeBasicInfoDto
                {
                    Id = employee.Id,
                    EmployeeNumber = employee.EmployeeNumber,
                    FirstName = employee.FirstName,
                    LastName = employee.LastName,
                    Email = employee.Email,
                    HireDate = employee.HireDate,
                    DepartmentId = employee.DepartmentId,
                },
                User = new UserDto
                {
                    Email = user.Email,
                    UserName = user.UserName
                },
                Role = role
            };

            return dto;
        }
    }
}

using Benefits.Application.Infrastructure.Contracts;
using Benefits.Common;
using Benefits.Domain;
using MediatR;
using System.Transactions;

namespace Benefits.Application.Features.Employees.CreateEmployeeWithUser
{
    public sealed class CreateEmployeeWithUserHandler : IRequestHandler<CreateEmployeeWithUserCommand, int>
    {
        private readonly IBenefitsDbContext _dbContext;
        private readonly IIdentityService _identityService;

        public CreateEmployeeWithUserHandler(IBenefitsDbContext dbContext, IIdentityService identityService)
        {
            _dbContext = Guard.NotNull(dbContext);
            _identityService = Guard.NotNull(identityService);
        }

        public async Task<int> Handle(CreateEmployeeWithUserCommand request, CancellationToken cancellationToken)
        {
            if (!await _identityService.RoleExistsAsync(request.Role))
            {
                throw new ArgumentException($"Role '{request.Role}' does not exist.");
            }

            if (await _identityService.UserExistsByEmaiAsync(request.User.Email))
            {
                throw new InvalidOperationException("A user with this email already exists.");
            }

            using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            var userId = await _identityService.CreateUserAsync(request.User.UserName, request.User.Email, request.User.Password);

            await _identityService.AssignRoleToUserAsync(userId, request.Role);

            var employee = new Employee
            {
                EmployeeNumber = request.Employee.EmployeeNumber,
                FirstName = request.Employee.FirstName,
                LastName = request.Employee.LastName,
                Email = request.Employee.Email,
                HireDate = request.Employee.HireDate,
                DepartmentId = request.Employee.DepartmentId
            };
            _dbContext.Employees.Add(employee);

            await _dbContext.SaveChangesAsync(cancellationToken);

            scope.Complete();

            return employee.Id;
        }
    }
}

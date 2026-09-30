using Benefits.Application.Application.Contracts;
using Benefits.Application.Features.Employees.CreateEmployeeWithUser;
using Benefits.Application.Infrastructure.Contracts;
using Benefits.Common;
using Benefits.Domain;

namespace Benefits.Application.Services;

public class ProvisioningService : IProvisioningService
{
    private readonly IBenefitsDbContext _dbContext;
    private readonly IIdentityService _identityService;

    public ProvisioningService(IBenefitsDbContext dbContext, IIdentityService identityService)
    {
        _dbContext = Guard.NotNull(dbContext);
        _identityService = Guard.NotNull(identityService);
    }

    public async Task<int> ProvisionEmployeeAsync(CreateEmployeeWithUserCommand request, CancellationToken cancellationToken)
    {
        // STEP 1 — Create process record (this is your durability anchor)
        var process = new EmployeeProvisioningProcess
        {
            Id = Guid.NewGuid(),
            Email = request.User.Email,
            UserName = request.User.UserName,
            Password = request.User.Password,
            Role = request.Role,
            Status = ProvisioningStatus.Started,
            CreatedAt = DateTime.UtcNow
        };

        _dbContext.ProvisioningProcesses.Add(process);
        await _dbContext.SaveChangesAsync(cancellationToken);

        try
        {
            // STEP 2 — Create user
            process.UserId = await _identityService.CreateUserAsync(
                process.UserName,
                process.Email,
                process.Password);

            process.Status = ProvisioningStatus.UserCreated;
            await _dbContext.SaveChangesAsync(cancellationToken);

            // STEP 3 — Assign role
            await _identityService.AssignRoleToUserAsync(process.UserId, process.Role);

            process.Status = ProvisioningStatus.RoleAssigned;
            await _dbContext.SaveChangesAsync(cancellationToken);

            // STEP 4 — Create Employee (CRITICAL LINK)
            var employee = new Employee
            {
                UserId = process.UserId, 
                EmployeeNumber = request.Employee.EmployeeNumber,
                FirstName = request.Employee.FirstName,
                LastName = request.Employee.LastName,
                Email = request.Employee.Email,
                HireDate = request.Employee.HireDate,
                DepartmentId = request.Employee.DepartmentId
            };

            _dbContext.Employees.Add(employee);
            await _dbContext.SaveChangesAsync(cancellationToken);

            process.EmployeeId = employee.Id;
            process.Status = ProvisioningStatus.EmployeeCreated;
            await _dbContext.SaveChangesAsync(cancellationToken);

            // STEP 5 — Complete
            process.Status = ProvisioningStatus.Completed;
            await _dbContext.SaveChangesAsync(cancellationToken);

            return employee.Id;
        }
        catch (Exception ex)
        {
            process.Status = ProvisioningStatus.Failed;
            process.Error = ex.Message;

            await _dbContext.SaveChangesAsync(cancellationToken);

            throw;
        }
    }
}
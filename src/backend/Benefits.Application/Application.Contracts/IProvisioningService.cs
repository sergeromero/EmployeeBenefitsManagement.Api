using Benefits.Application.Features.Employees.CreateEmployeeWithUser;

namespace Benefits.Application.Application.Contracts
{
    public interface IProvisioningService
    {
        Task<int> ProvisionEmployeeAsync(CreateEmployeeWithUserCommand request, CancellationToken cancellationToken);
    }
}

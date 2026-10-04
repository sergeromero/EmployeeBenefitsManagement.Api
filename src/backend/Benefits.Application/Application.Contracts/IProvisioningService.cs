using Benefits.Application.Features.Employees.CreateEmployeeWithUser;
using Benefits.Application.Features.Employees.UpdateEmployeeWithUser;

namespace Benefits.Application.Application.Contracts
{
    public interface IProvisioningService
    {
        Task<int> ProvisionEmployeeAsync(CreateEmployeeWithUserCommand request, CancellationToken cancellationToken);

        Task UpdateEmployeeAsync(UpdateEmployeeWithUserCommand request, CancellationToken cancellationToken);
    }
}

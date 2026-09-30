using Benefits.Application.Application.Contracts;
using Benefits.Common;
using MediatR;

namespace Benefits.Application.Features.Employees.CreateEmployeeWithUser
{
    public sealed class CreateEmployeeWithUserHandler : IRequestHandler<CreateEmployeeWithUserCommand, int>
    {
        private readonly IProvisioningService _provisioningService;

        public CreateEmployeeWithUserHandler(IProvisioningService provisioningService)
        {
            _provisioningService = Guard.NotNull(provisioningService);
        }

        public async Task<int> Handle(CreateEmployeeWithUserCommand request, CancellationToken cancellationToken)
        {
            return await _provisioningService.ProvisionEmployeeAsync(request, cancellationToken);
        }
    }
}

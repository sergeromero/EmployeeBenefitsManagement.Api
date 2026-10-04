using Benefits.Application.Application.Contracts;
using Benefits.Application.Exceptions;
using Benefits.Application.Infrastructure.Contracts;
using Benefits.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Benefits.Application.Features.Employees.UpdateEmployeeWithUser
{
    public sealed class UpdateEmployeeWithUserHandler : IRequestHandler<UpdateEmployeeWithUserCommand>
    {
        private readonly IProvisioningService _provisioningService;

        public UpdateEmployeeWithUserHandler(IProvisioningService provisioningService)
        {
            _provisioningService = Guard.NotNull(provisioningService);
        }

        public async Task Handle(UpdateEmployeeWithUserCommand request, CancellationToken cancellationToken)
        {
            await _provisioningService.UpdateEmployeeAsync(request, cancellationToken);
        }
    }
}

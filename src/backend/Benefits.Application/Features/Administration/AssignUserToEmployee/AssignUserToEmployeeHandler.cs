using Benefits.Application.Exceptions;
using Benefits.Application.Exceptions.BusinessRuleViolationException;
using Benefits.Application.Infrastructure.Contracts;
using Benefits.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Benefits.Application.Features.Administration.AssignUserToEmployee
{
    public sealed class AssignUserToEmployeeHandler : IRequestHandler<AssignUserToEmployeeCommand, Unit>
    {
        private readonly IIdentityService _identityService;
        private readonly IBenefitsDbContext _dbContext;

        public AssignUserToEmployeeHandler(IIdentityService identityService, IBenefitsDbContext dbContext)
        {
            _identityService = Guard.NotNull(identityService);
            _dbContext = Guard.NotNull(dbContext);
        }

        public async Task<Unit> Handle(AssignUserToEmployeeCommand request, CancellationToken cancellationToken)
        {
            var user = await _identityService.GetUserByIdAsync(request.UserId) ?? throw new NotFoundException("The user was not found.");

            var employee = await _dbContext.Employees.FirstAsync(e => e.Id == request.EmployeeId);

            if(employee.UserId != null)
            {
                throw new BusinessRuleException("Employee already has a user assigned.");
            }

            employee.UserId = request.UserId;
            await _dbContext.SaveChangesAsync();

            return Unit.Value;
        }
    }
}

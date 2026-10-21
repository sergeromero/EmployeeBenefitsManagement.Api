using Benefits.Application.Exceptions;
using Benefits.Application.Infrastructure.Contracts;
using Benefits.Common;
using MediatR;

namespace Benefits.Application.Features.Employees.Queries.ViewEmployeeBenefits
{
    public sealed class GetMyEmployeeBenefitsHandler : GetEmployeeBenefitsHandlerBase, IRequestHandler<GetMyEmployeeBenefitsQuery, IReadOnlyList<EmployeeBenefitDto>>
    {
        private readonly ICurrentUserService _currentUserService;

        public GetMyEmployeeBenefitsHandler(IBenefitsDbContext dbContext, ICurrentUserService currentUserService, TimeProvider timeProvider) : base(dbContext, timeProvider)
        {
            _currentUserService = Guard.NotNull(currentUserService);
        }

        public async Task<IReadOnlyList<EmployeeBenefitDto>> Handle(GetMyEmployeeBenefitsQuery request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;

            if (string.IsNullOrWhiteSpace(userId))
            {
                throw new NotFoundException("No active employee record is linked to the authenticated user.");
            }

            var employeeQuery = _dbContext.Employees.Where(e => e.UserId == userId && e.IsActive);

            var employeeBenefits = await GetEmployeeBenefitListAsync(employeeQuery, "No active employee record is linked to the authenticated user.", cancellationToken);

            return employeeBenefits;
        }
    }
}

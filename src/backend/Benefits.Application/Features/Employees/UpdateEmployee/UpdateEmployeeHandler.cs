using Benefits.Application.Exceptions;
using Benefits.Application.Infrastructure.Contracts;
using Benefits.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Benefits.Application.Features.Employees.UpdateEmployee
{
    public class UpdateEmployeeHandler : IRequestHandler<UpdateEmployeeCommand>
    {
        private readonly IBenefitsDbContext _dbContext;

        public UpdateEmployeeHandler(IBenefitsDbContext context)
        {
            _dbContext = Guard.NotNull(context);
        }

        public async Task Handle(UpdateEmployeeCommand request, CancellationToken cancellationToken)
        {
            var employee = await _dbContext.Employees
                .Where(e => e.Id == request.Id)
                .FirstOrDefaultAsync(cancellationToken) 
                ?? throw new NotFoundException($"Employee with Id {request.Id} was not found.");

            employee.EmployeeNumber = request.EmployeeNumber;
            employee.FirstName = request.FirstName;
            employee.LastName = request.LastName;
            employee.Email  = request.Email;
            employee.HireDate = request.HireDate;
            employee.DepartmentId = request.DepartmentId;

            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}

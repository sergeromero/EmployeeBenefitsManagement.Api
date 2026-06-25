using Benefits.Application.Infrastructure.Contracts;
using Benefits.Common;
using Domain;
using MediatR;

namespace Benefits.Application.Features.Employees.CreateEmployee
{
    public class CreateEmployeeHandler : IRequestHandler<CreateEmployeeCommand, int>
    {
        private readonly IBenefitsDbContext _dbContext;

        public CreateEmployeeHandler(IBenefitsDbContext dbContext)
        {
            _dbContext = Guard.NotNull(dbContext);
        }

        public async Task<int> Handle(CreateEmployeeCommand request, CancellationToken cancellationToken)
        {
            var employee = new Employee
            {
                EmployeeNumber = int.Parse(request.EmployeeNumber),
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                DepartmentId = request.DepartmentId 
            };

            _dbContext.Employees.Add(employee);

            await _dbContext.SaveChangesAsync(cancellationToken);

            return employee.Id;
        }
    }
}

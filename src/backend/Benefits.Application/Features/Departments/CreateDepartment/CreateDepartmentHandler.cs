using Benefits.Application.Infrastructure.Contracts;
using Benefits.Common;
using Benefits.Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Benefits.Application.Features.Departments.CreateDepartment
{
    public sealed class CreateDepartmentHandler : IRequestHandler<CreateDepartmentCommand, int>
    {
        private readonly IBenefitsDbContext _dbContext;

        public CreateDepartmentHandler(IBenefitsDbContext dbContext)
        {
            _dbContext = Guard.NotNull(dbContext);
        }

        public async Task<int> Handle(CreateDepartmentCommand request, CancellationToken cancellationToken)
        {
            var exists = await _dbContext.Departments.FirstOrDefaultAsync(d => d.Name == request.Name, cancellationToken);

            if (exists != null)
            {
                return exists.Id;
            }

            var department = new Department { Name = request.Name };

            _dbContext.Departments.Add(department);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return department.Id;
        }
    }
}

using Domain;
using Microsoft.EntityFrameworkCore;

namespace Benefits.Application.Infrastructure.Contracts
{
    public interface IBenefitsDbContext
    {
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

        public DbSet<Department> Departments { get; set; }
        public DbSet<Employee> Employees { get; set; }
    }
}

using Benefits.Application.Infrastructure.Contracts;
using Benefits.Domain;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace Benefits.Infrastructure.Persistance
{
    internal class BenefitsDbContext : DbContext, IBenefitsDbContext
    {
        public BenefitsDbContext(DbContextOptions<BenefitsDbContext> options) : base(options)
        {}

        public DbSet<Department> Departments { get; set; } = null!;
        public DbSet<Employee> Employees { get; set; } = null!;

        public DbSet<BenefitType> BenefitTypes { get; set; } = null!;
    }
}

using Benefits.Application.Infrastructure.Contracts;
using Benefits.Domain;
using Microsoft.EntityFrameworkCore;

namespace Benefits.Infrastructure.Persistence
{
    internal sealed class BenefitsDbContext : DbContext, IBenefitsDbContext
    {
        public BenefitsDbContext(DbContextOptions<BenefitsDbContext> options) : base(options)
        {}

        public DbSet<Employee> Employees { get; set; } = default!;
        public DbSet<Department> Departments { get; set; } = default!;

        public DbSet<BenefitType> BenefitTypes { get; set; } = default!;

        public DbSet<EnrollmentCategory> EnrollmentCategories { get; set; } = default!;

        public DbSet<BenefitPlan> BenefitPlans { get; set; } = default!;

        public DbSet<EmployeeEnrollment> EmployeeEnrollments { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(
                typeof(BenefitsDbContext).Assembly);
        }
    }
}

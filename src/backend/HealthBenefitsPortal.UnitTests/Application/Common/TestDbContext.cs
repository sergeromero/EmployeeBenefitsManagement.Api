using Benefits.Application.Infrastructure.Contracts;
using Benefits.Domain;
using Microsoft.EntityFrameworkCore;

namespace HealthBenefitsPortal.UnitTests.Application.Common
{
    public class TestDbContext : DbContext, IBenefitsDbContext
    {
        public TestDbContext(DbContextOptions<TestDbContext> options) : base(options)
        {            
        }

        public DbSet<Employee> Employees { get; set; } = null!;
        public DbSet<Department> Departments { get; set; } = null!;
        public DbSet<BenefitType> BenefitTypes { get; set; } = null!;
        public DbSet<EnrollmentCategory> EnrollmentCategories { get; set; } = null!;
        public DbSet<BenefitPlan> BenefitPlans { get; set; } = null!;
        public DbSet<EmployeeEnrollment> EmployeeEnrollments { get; set; } = null!;

        public DbSet<EmployeeProvisioningProcess> ProvisioningProcesses => Set<EmployeeProvisioningProcess>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<EmployeeEnrollment>()
                .HasKey(enrollment => new
                {
                    enrollment.EmployeeId,
                    enrollment.BenefitPlanId
                });
        }
    }
}

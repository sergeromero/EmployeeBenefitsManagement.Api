using Benefits.Domain;
using Microsoft.EntityFrameworkCore;

namespace Benefits.Application.Infrastructure.Contracts
{
    public interface IBenefitsDbContext
    {
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

        public DbSet<Employee> Employees { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<BenefitType> BenefitTypes { get; set; }
        public DbSet<EnrollmentCategory> EnrollmentCategories { get; set; }
        public DbSet<BenefitPlan> BenefitPlans { get; set; }
        public DbSet<EmployeeEnrollment> EmployeeEnrollments { get; set; }
    }
}

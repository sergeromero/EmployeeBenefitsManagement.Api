using Benefits.Application.Infrastructure.Contracts;
using Benefits.Domain;
using Benefits.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Benefits.Infrastructure.Persistence
{
    internal sealed class BenefitsDbContext : IdentityDbContext<ApplicationUser>, IBenefitsDbContext
    {
        private const string TableIdentityUsers = "IdentityUsers";
        private const string TableIdentityRoles = "IdentityRoles";
        private const string TableIdentityUserRoles = "IdentityUserRoles";
        private const string TableIdentityUserClaims = "IdentityUserClaims";
        private const string TableIdentityUserLogins = "IdentityUserLogins";
        private const string TableIdentityRoleClaims = "IdentityRoleClaims";
        private const string TableIdentityUserTokens = "IdentityUserTokens";

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

            modelBuilder.Entity<ApplicationUser>().ToTable(TableIdentityUsers);
            modelBuilder.Entity<IdentityRole>().ToTable(TableIdentityRoles);
            modelBuilder.Entity<IdentityUserRole<string>>().ToTable(TableIdentityUserRoles);
            modelBuilder.Entity<IdentityUserClaim<string>>().ToTable(TableIdentityUserClaims);
            modelBuilder.Entity<IdentityUserLogin<string>>().ToTable(TableIdentityUserLogins);
            modelBuilder.Entity<IdentityRoleClaim<string>>().ToTable(TableIdentityRoleClaims);
            modelBuilder.Entity<IdentityUserToken<string>>().ToTable(TableIdentityUserTokens);

            modelBuilder.ApplyConfigurationsFromAssembly(
                typeof(BenefitsDbContext).Assembly);
        }
    }
}

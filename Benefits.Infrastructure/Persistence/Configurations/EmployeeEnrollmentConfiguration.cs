using Benefits.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Benefits.Infrastructure.Persistence.Configurations
{
    internal sealed class EmployeeEnrollmentConfiguration : IEntityTypeConfiguration<EmployeeEnrollment>
    {
        public void Configure(EntityTypeBuilder<EmployeeEnrollment> builder)
        {
            builder.HasKey(x => new
            {
                x.EmployeeId,
                x.BenefitPlanId
            });
        }
    }
}

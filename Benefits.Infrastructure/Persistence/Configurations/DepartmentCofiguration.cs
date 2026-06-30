using Benefits.Domain;
using Benefits.Domain.ReferenceData;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Benefits.Infrastructure.Persistence.Configurations
{
    internal sealed class DepartmentConfiguration : IEntityTypeConfiguration<Department>
    {
        public void Configure(EntityTypeBuilder<Department> builder)
        {
            builder.ToTable("Departments");

            builder.Property(x => x.Name)
                .HasMaxLength(100)
                .IsRequired();

            builder.HasData(
                new { Id = DepartmentIds.Finance, Name = "Finance" },
                new { Id = DepartmentIds.Engineering, Name = "Engineering" },
                new { Id = DepartmentIds.Marketing, Name = "Marketing" },
                new { Id = DepartmentIds.HumanResources, Name = "Human Resources" },
                new { Id = DepartmentIds.Sales, Name = "Sales" });
        }
    }
}

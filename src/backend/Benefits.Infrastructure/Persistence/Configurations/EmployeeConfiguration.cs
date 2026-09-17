using Benefits.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Benefits.Infrastructure.Persistence.Configurations
{
    internal sealed class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
    {
        public void Configure(EntityTypeBuilder<Employee> builder)
        {
            builder.ToTable("Employees");

            builder.Property(x => x.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            builder.Property(x => x.EmployeeNumber)
                .HasMaxLength(8)
                .IsFixedLength()
                .IsRequired();

            builder.Property(x => x.FirstName)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(x => x.LastName)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(x => x.Email)
                .HasMaxLength(150)
                .IsRequired();

            builder.Property(x => x.HireDate)
                .IsRequired();

            builder.Property(x => x.DepartmentId)
                .IsRequired();

            builder.HasIndex(x => x.EmployeeNumber)
                .IsUnique();

            builder.HasIndex(x => x.Email)
                .IsUnique();

            builder.HasOne(e => e.Department)
                .WithMany(d => d.Employees)
                .HasForeignKey(e => e.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(e => e.Enrollments)
                .WithOne(ee => ee.Employee)
                .HasForeignKey(ee => ee.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

using Benefits.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class EmployeeProvisioningProcessConfiguration
    : IEntityTypeConfiguration<EmployeeProvisioningProcess>
{
    public void Configure(EntityTypeBuilder<EmployeeProvisioningProcess> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Email).IsRequired();
        builder.Property(x => x.UserName).IsRequired();
        builder.Property(x => x.Password).IsRequired();
        builder.Property(x => x.Role).IsRequired();

        builder.Property(x => x.Status).HasConversion<int>();

        builder.Property(x => x.CreatedAt).IsRequired();
    }
}
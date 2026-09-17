using Benefits.Domain;
using Benefits.Domain.ReferenceData;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Benefits.Infrastructure.Persistence.Configurations
{
    internal sealed class EnrollmentCategoryConfiguration : IEntityTypeConfiguration<EnrollmentCategory>
    {
        public void Configure(EntityTypeBuilder<EnrollmentCategory> builder)
        {
            builder.ToTable("EnrollmentCategories");

            builder.Property(x => x.Name)
                .HasMaxLength(50)
                .IsRequired();

            builder.HasData(
                new { Id = EnrollmentCategoryIds.HealthCore, Name = "Health Core" },
                new { Id = EnrollmentCategoryIds.DentalCore, Name = "Dental Core" },
                new { Id = EnrollmentCategoryIds.Orthodontics, Name = "Orthodontics" },
                new { Id = EnrollmentCategoryIds.Vision, Name = "Vision" },
                new { Id = EnrollmentCategoryIds.LifeInsurance, Name = "Life Insurance" });
        }
    }
}

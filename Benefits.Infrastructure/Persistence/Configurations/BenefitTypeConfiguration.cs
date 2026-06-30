using Benefits.Domain;
using Benefits.Domain.ReferenceData;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Benefits.Infrastructure.Persistence.Configurations
{
    internal sealed class BenefitTypeConfiguration : IEntityTypeConfiguration<BenefitType>
    {
        public void Configure(EntityTypeBuilder<BenefitType> builder)
        {
            builder.ToTable("BenefitTypes");

            builder.Property(x => x.Name)
                .HasMaxLength(30)
                .IsRequired();

            builder.HasData(
                new { Id = BenefitTypeIds.Health, Name = "Health" },
                new { Id = BenefitTypeIds.Dental, Name = "Dental" },
                new { Id = BenefitTypeIds.Vision, Name = "Vision" },
                new { Id = BenefitTypeIds.Life, Name = "Life Insurance" });
        }
    }
}

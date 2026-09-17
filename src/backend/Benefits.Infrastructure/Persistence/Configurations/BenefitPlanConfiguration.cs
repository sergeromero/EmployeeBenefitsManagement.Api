using Benefits.Domain;
using Benefits.Domain.ReferenceData;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Benefits.Infrastructure.Persistence.Configurations
{
    internal sealed class BenefitPlanConfiguration : IEntityTypeConfiguration<BenefitPlan>
    {
        public void Configure(EntityTypeBuilder<BenefitPlan> builder)
        {
            builder.ToTable("BenefitPlans");

            builder.Property(x => x.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            builder.Property(x => x.Name)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(x => x.Description)
                .HasMaxLength(200)
                .IsRequired();

            builder
                .HasOne(bp => bp.BenefitType)
                .WithMany(bt => bt.BenefitPlans)
                .HasForeignKey(bp => bp.BenefitTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(bp => bp.EnrollmentCategory)
                .WithMany(ec => ec.BenefitPlans)
                .HasForeignKey(bp => bp.EnrollmentCategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(bp => bp.Enrollments)
                .WithOne(ee => ee.BenefitPlan)
                .HasForeignKey(ee => ee.BenefitPlanId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasData(
                new { Id = 1, Name = "Health Basic", BenefitTypeId = BenefitTypeIds.Health, EnrollmentCategoryId = EnrollmentCategoryIds.HealthCore, Description = "Basic medical coverage." },
                new { Id = 2, Name = "Health Premium", BenefitTypeId = BenefitTypeIds.Health, EnrollmentCategoryId = EnrollmentCategoryIds.HealthCore, Description = "Enhanced medical coverage with lower deductibles." },
                new { Id = 3, Name = "Health Family", BenefitTypeId = BenefitTypeIds.Health, EnrollmentCategoryId = EnrollmentCategoryIds.HealthCore, Description = "Medical coverage for employee and dependents." },
                new { Id = 4, Name = "Dental Basic", BenefitTypeId = BenefitTypeIds.Dental, EnrollmentCategoryId = EnrollmentCategoryIds.DentalCore, Description = "Preventive and restorative dental care." },
                new { Id = 5, Name = "Dental Premium", BenefitTypeId = BenefitTypeIds.Dental, EnrollmentCategoryId = EnrollmentCategoryIds.DentalCore, Description = "Extended dental coverage including major procedures." },
                new { Id = 6, Name = "Orthodontic Coverage", BenefitTypeId = BenefitTypeIds.Dental, EnrollmentCategoryId = EnrollmentCategoryIds.Orthodontics, Description = "Orthodontic treatment coverage for eligible dependents." },
                new { Id = 7, Name = "Vision Basic", BenefitTypeId = BenefitTypeIds.Vision, EnrollmentCategoryId = EnrollmentCategoryIds.Vision, Description = "Annual eye exams and prescription lenses." },
                new { Id = 8, Name = "Vision Premium", BenefitTypeId = BenefitTypeIds.Vision, EnrollmentCategoryId = EnrollmentCategoryIds.Vision, Description = "Enhanced vision benefits with higher reimbursement limits." },
                new { Id = 9, Name = "Life Insurance Basic", BenefitTypeId = BenefitTypeIds.Life, EnrollmentCategoryId = EnrollmentCategoryIds.LifeInsurance, Description = "Employer-paid life insurance coverage." },
                new { Id = 10, Name = "Life Insurance Enhanced", BenefitTypeId = BenefitTypeIds.Life, EnrollmentCategoryId = EnrollmentCategoryIds.LifeInsurance, Description = "Optional supplemental life insurance." });
        }
    }
}

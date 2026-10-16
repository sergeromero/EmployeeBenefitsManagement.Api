using Benefits.Application.Features.BenefitPlans.Queries.GetBenefitPlans;
using Benefits.Domain;
using FluentAssertions;
using HealthBenefitsPortal.UnitTests.Application.Common;
using Microsoft.EntityFrameworkCore;

namespace HealthBenefitsPortal.UnitTests.Application.Features.BenefitPlans.Queries.GetBenefitPlans
{
    public sealed class GetBenefitsPlansHandlerTests
    {

        [Fact]
        public async Task Handle_WhenActiveAndInactivePlansExist_ReturnsOnlyActivePlans()
        {
            await using var dbContext = CreateDbContext();

            var benefitType = new BenefitType { Name = "Health" };
            var enrollmentCategory = new EnrollmentCategory { Name = "Health Benefits" };

            dbContext.BenefitTypes.Add(benefitType);
            dbContext.EnrollmentCategories.Add(enrollmentCategory);

            dbContext.BenefitPlans.AddRange(
                new BenefitPlan
                {
                    Name = "Active Plan",
                    Description = "An active benefit plan.",
                    IsActive = true,
                    BenefitType = benefitType,
                    EnrollmentCategory = enrollmentCategory
                },
                new BenefitPlan
                {
                    Name = "Inactive Plan",
                    Description = "An inactive benefit plan.",
                    IsActive = false,
                    BenefitType = benefitType,
                    EnrollmentCategory = enrollmentCategory
                });

            await dbContext.SaveChangesAsync(CancellationToken.None);

            var handler = new GetBenefitPlansHandler(dbContext);

            var result = await handler.Handle(
                new GetBenefitPlansQuery(),
                CancellationToken.None);

            result.Should().ContainSingle();

            result[0].Name.Should().Be("Active Plan");
            result[0].Description.Should().Be("An active benefit plan.");
        }

        [Fact]
        public async Task Handle_WhenMultipleActivePlansExist_OrdersByBenefitTypeThenPlanName()
        {
            await using var dbContext = CreateDbContext();

            var dental = new BenefitType { Name = "Dental" };
            var health = new BenefitType { Name = "Health" };
            var category = new EnrollmentCategory { Name = "Standard Benefits" };

            dbContext.BenefitTypes.AddRange(dental, health);
            dbContext.EnrollmentCategories.Add(category);

            dbContext.BenefitPlans.AddRange(
                CreateBenefitPlan("Premium", dental, category),
                CreateBenefitPlan("Basic", health, category),
                CreateBenefitPlan("Basic", dental, category),
                CreateBenefitPlan("Premium", health, category));

            await dbContext.SaveChangesAsync(CancellationToken.None);

            var handler = new GetBenefitPlansHandler(dbContext);

            var result = await handler.Handle(new GetBenefitPlansQuery(), CancellationToken.None);

            result.Select(plan => (plan.BenefitType.Name, plan.Name))
                .Should()
                .Equal(
                    ("Dental", "Basic"),
                    ("Dental", "Premium"),
                    ("Health", "Basic"),
                    ("Health", "Premium"));
        }

        [Fact]
        public async Task Handle_WhenActivePlansExist_MapsAllExpectedDtoProperties()
        {
            await using var dbContext = CreateDbContext();

            var benefitType = new BenefitType { Name = "Dental" };
            var enrollmentCategory = new EnrollmentCategory { Name = "Dental Benefits" };

            var benefitPlan = new BenefitPlan
            {
                Name = "Enhanced Dental",
                Description = "Coverage for eligible dental expenses.",
                IsActive = true,
                BenefitType = benefitType,
                EnrollmentCategory = enrollmentCategory
            };

            dbContext.BenefitPlans.Add(benefitPlan);

            await dbContext.SaveChangesAsync(CancellationToken.None);

            var handler = new GetBenefitPlansHandler(dbContext);

            var result = await handler.Handle(new GetBenefitPlansQuery(), CancellationToken.None);

            result.Should().ContainSingle();

            var item = result[0];

            item.Id.Should().Be(benefitPlan.Id);
            item.Name.Should().Be("Enhanced Dental");
            item.Description.Should().Be("Coverage for eligible dental expenses.");
            item.BenefitType.Id.Should().Be(benefitType.Id);
            item.BenefitType.Name.Should().Be("Dental");
            item.EnrollmentCategoryName.Should().Be("Dental Benefits");
        }

        [Fact]
        public async Task Handle_WhenNoActivePlansExist_ReturnsEmptyCollection()
        {
            await using var dbContext = CreateDbContext();

            var benefitType = new BenefitType { Name = "Health" };
            var category = new EnrollmentCategory { Name = "Health Benefits" };

            dbContext.BenefitPlans.Add(
                CreateBenefitPlan("Inactive Plan", benefitType, category, isActive: false));

            await dbContext.SaveChangesAsync(CancellationToken.None);

            var handler = new GetBenefitPlansHandler(dbContext);

            var result = await handler.Handle( new GetBenefitPlansQuery(), CancellationToken.None);

            result.Should().BeEmpty();
        }

        private static BenefitPlan CreateBenefitPlan(
            string name,
            BenefitType benefitType,
            EnrollmentCategory enrollmentCategory,
            bool isActive = true)
        {
            return new BenefitPlan
            {
                Name = name,
                Description = $"{name} description.",
                IsActive = isActive,
                BenefitType = benefitType,
                EnrollmentCategory = enrollmentCategory
            };
        }

        private static TestDbContext CreateDbContext()
        {
            var options = new DbContextOptionsBuilder<TestDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new TestDbContext(options);
        }
    }
}

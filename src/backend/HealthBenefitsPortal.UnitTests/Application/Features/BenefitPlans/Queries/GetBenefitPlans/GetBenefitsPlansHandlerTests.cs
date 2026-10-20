using Benefits.Application.Exceptions;
using Benefits.Application.Features.BenefitPlans.Queries.GetBenefitPlans;
using Benefits.Application.Infrastructure.Contracts;
using Benefits.Domain;
using FluentAssertions;
using HealthBenefitsPortal.UnitTests.Application.Common;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace HealthBenefitsPortal.UnitTests.Application.Features.BenefitPlans.Queries.GetBenefitPlans
{
    public sealed class GetBenefitsPlansHandlerTests
    {
        const string ANY_USER_ID = "any-user-id";
        const int ANY_EMPLOYEE_ID = 583;

        private Employee anyEmployee = new Employee
        {
            Id = ANY_EMPLOYEE_ID,
            FirstName = "First Name",
            LastName = "Last Name",
            Email = "anyemail@email.com",
            DepartmentId = 1,
            EmployeeNumber = "EMP00001",
            HireDate = new DateOnly(2025, 12, 12),
            IsActive = true,
            UserId = ANY_USER_ID
        };

        [Fact]
        public async Task Handle_WhenActiveAndInactivePlansExist_ReturnsOnlyActivePlans()
        {
            await using var dbContext = CreateDbContext();
            var currentUserService = new Mock<ICurrentUserService>();

            currentUserService.Setup(service => service.UserId).Returns(ANY_USER_ID);
            dbContext.Employees.Add(anyEmployee);
            
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

            var handler = new GetBenefitPlansHandler(dbContext, currentUserService.Object);

            var result = await handler.Handle(
                new GetBenefitPlansQuery(),
                CancellationToken.None);

            result.Should().ContainSingle();

            result[0].Name.Should().Be("Active Plan");
            result[0].Description.Should().Be("An active benefit plan.");
            result[0].EnrollmentEligibility.Should().Be(new EnrollmentEligibilityDto(true, null));
        }

        [Fact]
        public async Task Handle_WhenMultipleActivePlansExist_OrdersByBenefitTypeThenPlanName()
        {
            await using var dbContext = CreateDbContext();
            var currentUserService = new Mock<ICurrentUserService>();

            currentUserService.Setup(service => service.UserId).Returns(ANY_USER_ID);
            dbContext.Employees.Add(anyEmployee);

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

            var handler = new GetBenefitPlansHandler(dbContext, currentUserService.Object);

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
            var currentUserService = new Mock<ICurrentUserService>();

            currentUserService.Setup(service => service.UserId).Returns(ANY_USER_ID);
            dbContext.Employees.Add(anyEmployee);

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

            var handler = new GetBenefitPlansHandler(dbContext, currentUserService.Object);

            var result = await handler.Handle(new GetBenefitPlansQuery(), CancellationToken.None);

            result.Should().ContainSingle();

            var item = result[0];

            item.Id.Should().Be(benefitPlan.Id);
            item.Name.Should().Be("Enhanced Dental");
            item.Description.Should().Be("Coverage for eligible dental expenses.");
            item.BenefitType.Id.Should().Be(benefitType.Id);
            item.BenefitType.Name.Should().Be("Dental");
            item.EnrollmentCategoryName.Should().Be("Dental Benefits");
            item.EnrollmentEligibility.Should().Be(new EnrollmentEligibilityDto(true, null));
        }

        [Fact]
        public async Task Handle_WhenNoActivePlansExist_ReturnsEmptyCollection()
        {
            await using var dbContext = CreateDbContext();
            var currentUserService = new Mock<ICurrentUserService>();

            currentUserService.Setup(service => service.UserId).Returns(ANY_USER_ID);
            dbContext.Employees.Add(anyEmployee);

            var benefitType = new BenefitType { Name = "Health" };
            var category = new EnrollmentCategory { Name = "Health Benefits" };

            dbContext.BenefitPlans.Add(
                CreateBenefitPlan("Inactive Plan", benefitType, category, isActive: false));

            await dbContext.SaveChangesAsync(CancellationToken.None);

            var handler = new GetBenefitPlansHandler(dbContext, currentUserService.Object);

            var result = await handler.Handle( new GetBenefitPlansQuery(), CancellationToken.None);

            result.Should().BeEmpty();
        }

        [Fact]
        public async Task Handle_WhenEmployeeIsActivelyEnrolledInPlan_MarksPlanAsAlreadyEnrolledInBenefit()
        {
            await using var dbContext = CreateDbContext();
            var currentUserService = new Mock<ICurrentUserService>();

            currentUserService.Setup(service => service.UserId).Returns(ANY_USER_ID);
            dbContext.Employees.Add(anyEmployee);

            var benefitType = new BenefitType { Name = "Health" };
            var category = new EnrollmentCategory { Name = "Health Benefits" };
            var plan = CreateBenefitPlan("Health Plan", benefitType, category);
            dbContext.BenefitPlans.Add(plan);
            await dbContext.SaveChangesAsync(CancellationToken.None);

            AddEnrollment(dbContext, anyEmployee, plan);
            await dbContext.SaveChangesAsync(CancellationToken.None);

            var handler = new GetBenefitPlansHandler(dbContext, currentUserService.Object);
            var result = await handler.Handle(new GetBenefitPlansQuery(), CancellationToken.None);

            result.Should().ContainSingle();
            result[0].EnrollmentEligibility.Should().Be(
                new EnrollmentEligibilityDto(false, EnrollmentIneligibilityReason.AlreadyEnrolledInBenefit));
        }

        [Fact]
        public async Task Handle_WhenEmployeeIsEnrolledInDifferentPlanInSameCategory_MarksPlanAsAlreadyEnrolledInCategory()
        {
            await using var dbContext = CreateDbContext();
            var currentUserService = new Mock<ICurrentUserService>();

            currentUserService.Setup(service => service.UserId).Returns(ANY_USER_ID);
            dbContext.Employees.Add(anyEmployee);

            var benefitType = new BenefitType { Name = "Health" };
            var category = new EnrollmentCategory { Name = "Health Benefits" };
            var existingPlan = CreateBenefitPlan("Basic Health", benefitType, category);
            var availableCataloguePlan = CreateBenefitPlan("Premium Health", benefitType, category);

            dbContext.BenefitPlans.AddRange(existingPlan, availableCataloguePlan);
            await dbContext.SaveChangesAsync(CancellationToken.None);
            AddEnrollment(dbContext, anyEmployee, existingPlan);
            await dbContext.SaveChangesAsync(CancellationToken.None);

            var handler = new GetBenefitPlansHandler(dbContext, currentUserService.Object);
            var result = await handler.Handle(new GetBenefitPlansQuery(), CancellationToken.None);

            result.Should().HaveCount(2);
            result.Single(plan => plan.Id == existingPlan.Id).EnrollmentEligibility.Should().Be(
                new EnrollmentEligibilityDto(false, EnrollmentIneligibilityReason.AlreadyEnrolledInBenefit));
            result.Single(plan => plan.Id == availableCataloguePlan.Id).EnrollmentEligibility.Should().Be(
                new EnrollmentEligibilityDto(false, EnrollmentIneligibilityReason.AlreadyEnrolledInCategory));
        }

        [Fact]
        public async Task Handle_WhenOnlyTerminatedEnrollmentExists_AllowsEnrollmentInThatCategory()
        {
            await using var dbContext = CreateDbContext();
            var currentUserService = new Mock<ICurrentUserService>();

            currentUserService.Setup(service => service.UserId).Returns(ANY_USER_ID);
            dbContext.Employees.Add(anyEmployee);

            var benefitType = new BenefitType { Name = "Health" };
            var category = new EnrollmentCategory { Name = "Health Benefits" };
            var existingPlan = CreateBenefitPlan("Basic Health", benefitType, category);
            var cataloguePlan = CreateBenefitPlan("Premium Health", benefitType, category);

            dbContext.BenefitPlans.AddRange(existingPlan, cataloguePlan);
            await dbContext.SaveChangesAsync(CancellationToken.None);

            var enrollment = AddEnrollment(dbContext, anyEmployee, existingPlan);
            enrollment.EndEnrollment(DateOnly.FromDateTime(DateTime.UtcNow));
            await dbContext.SaveChangesAsync(CancellationToken.None);

            var handler = new GetBenefitPlansHandler(dbContext, currentUserService.Object);
            var result = await handler.Handle(new GetBenefitPlansQuery(), CancellationToken.None);

            result.Single(plan => plan.Id == cataloguePlan.Id).EnrollmentEligibility.Should().Be(
                new EnrollmentEligibilityDto(true, null));
        }

        [Fact]
        public async Task Handle_WhenCurrentUserHasNoActiveEmployee_ThrowsNotFoundException()
        {
            await using var dbContext = CreateDbContext();
            var currentUserService = new Mock<ICurrentUserService>();

            currentUserService.Setup(service => service.UserId).Returns(ANY_USER_ID);

            var handler = new GetBenefitPlansHandler(dbContext, currentUserService.Object);

            var act = () => handler.Handle(new GetBenefitPlansQuery(), CancellationToken.None);

            await act.Should().ThrowAsync<NotFoundException>()
                .WithMessage("No active employee record is linked to the authenticated user.");
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

        private static EmployeeEnrollment AddEnrollment(TestDbContext dbContext, Employee employee, BenefitPlan plan)
        {
            var enrollment = new EmployeeEnrollment
            {
                EmployeeId = employee.Id,
                Employee = employee,
                BenefitPlanId = plan.Id,
                BenefitPlan = plan,
                EnrollmentDate = DateOnly.FromDateTime(DateTime.UtcNow)
            };

            dbContext.EmployeeEnrollments.Add(enrollment);
            return enrollment;
        }
    }
}

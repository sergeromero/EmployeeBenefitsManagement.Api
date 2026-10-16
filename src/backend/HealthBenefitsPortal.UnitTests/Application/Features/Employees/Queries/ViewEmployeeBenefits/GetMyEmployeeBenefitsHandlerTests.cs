using Benefits.Application.Exceptions;
using Benefits.Application.Features.Employees.Queries.ViewEmployeeBenefits;
using Benefits.Application.Infrastructure.Contracts;
using Benefits.Domain;
using FluentAssertions;
using HealthBenefitsPortal.UnitTests.Application.Common;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace HealthBenefitsPortal.UnitTests.Application.Features.Employees.Queries.ViewEmployeeBenefits
{
    public class GetMyEmployeeBenefitsHandlerTests
    {
        DbContextOptions<TestDbContext> _options;
        public GetMyEmployeeBenefitsHandlerTests()
        {
            _options = new DbContextOptionsBuilder<TestDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
        }
        [Fact]
        public async Task Handle_WhenCurrentUserHasActiveEmployee_ReturnsTheirBenefits()
        {
            await using var dbContext = new TestDbContext(_options);

            var currentUserId = "user-123";

            var department = new Department { Id = 1, Name = "Engineering" };
            var benefitType = new BenefitType { Id = 1, Name = "Health" };
            var enrollmentCategory = new EnrollmentCategory { Id = 1, Name = "Health Benefits" };

            var benefitPlan = new BenefitPlan
            {
                Id = 10,
                Name = "Extended Health",
                Description = " Extended health coverage",
                IsActive = true,
                BenefitTypeId = benefitType.Id,
                BenefitType = benefitType,
                EnrollmentCategoryId = enrollmentCategory.Id,
                EnrollmentCategory = enrollmentCategory
            };

            var employee = new Employee
            {
                Id = 100,
                EmployeeNumber = "EMP00001",
                FirstName = "First Name",
                LastName = "Last Name",
                Email = "anyemail@email.com",
                HireDate = new DateOnly(2024, 1, 1),
                DepartmentId = department.Id,
                Department = department,
                IsActive = true,
                UserId = currentUserId,
            };

            var enrollment = new EmployeeEnrollment
            {
                EmployeeId = employee.Id,
                BenefitPlanId = benefitPlan.Id,
                EnrollmentDate = new DateOnly(2025, 6, 6),
                Employee = employee,
                BenefitPlan = benefitPlan
            };

            employee.Enrollments = new List<EmployeeEnrollment> { enrollment };

            var currentUserService = new Mock<ICurrentUserService>();
            currentUserService.Setup(service => service.UserId).Returns(currentUserId);

            await dbContext.Employees.AddAsync(employee, CancellationToken.None);
            await dbContext.SaveChangesAsync(CancellationToken.None);

            var handler = new GetMyEmployeeBenefitsHandler(dbContext, currentUserService.Object);

            var result = await handler.Handle(new GetMyEmployeeBenefitsQuery(), CancellationToken.None);

            result.Id.Should().Be(employee.Id);
            result.FirstName.Should().Be(employee.FirstName);
            result.LastName.Should().Be(employee.LastName);
            result.Department.Should().Be(department.Name);

            result.Benefits.Should().ContainSingle();

            var returnedBenefit = result.Benefits[0];

            returnedBenefit.BenefitPlanId.Should().Be(benefitPlan.Id);
            returnedBenefit.BenefitPlan.Should().Be(benefitPlan.Name);
            returnedBenefit.BenefitType.Should().Be(benefitType.Name);
            returnedBenefit.EnrollmentDate.Should().Be(enrollment.EnrollmentDate);
            returnedBenefit.EndDate.Should().BeNull();
            returnedBenefit.IsActive.Should().BeTrue();

            currentUserService.VerifyGet(
                service => service.UserId,
                Times.Once);
        }

        [Fact]
        public async Task Handle_WhenUserIdIsMissing_ThrowsNotFoundException()
        {
            await using var dbContext = new TestDbContext(_options);

            var currentUserService = new Mock<ICurrentUserService>();
            currentUserService.Setup(service => service.UserId).Returns((string?)null);

            var handler = new GetMyEmployeeBenefitsHandler(dbContext, currentUserService.Object);

            var act = () => handler.Handle(new GetMyEmployeeBenefitsQuery(), CancellationToken.None);

            await act.Should().ThrowAsync<NotFoundException>()
                .WithMessage("No active employee record is linked to the authenticated user.");

            currentUserService.VerifyGet(service => service.UserId, Times.Once);
        }

        [Fact]
        public async Task Handle_WhenUserHasNotAnAssociatedEmployee_ThrowsNotFoundException()
        {
            await using var dbContext = new TestDbContext(_options);

            var currentUserId = "user-123"; 
            var employee = new Employee 
            { 
                Id = 100, 
                EmployeeNumber = "EMP10001", 
                FirstName = "First Name", 
                LastName = "Last Name", 
                Email = "anyemail@email.com", 
                HireDate = new DateOnly(2025, 1, 1), 
                DepartmentId = 1, 
                Department = new Department { Id = 1, Name = "Engineering" },
                UserId = currentUserId, 
                IsActive = false 
            };

            await dbContext.Employees.AddAsync(employee, CancellationToken.None);
            await dbContext.SaveChangesAsync(CancellationToken.None);

            var currentUserService = new Mock<ICurrentUserService>();
            currentUserService.Setup(service => service.UserId).Returns(currentUserId);

            var handler = new GetMyEmployeeBenefitsHandler(dbContext, currentUserService.Object);

            var act = () => handler.Handle(new GetMyEmployeeBenefitsQuery(), CancellationToken.None);

            await act.Should().ThrowAsync<NotFoundException>()
                .WithMessage("No active employee record is linked to the authenticated user.");

            currentUserService.VerifyGet(service => service.UserId, Times.Once);
        }

        [Fact]
        public async Task Handle_WhenMultipleEmployeesExist_ReturnsEmployeeLinkedToCurrentUser()
        {
            await using var dbContext = new TestDbContext(_options);

            const string currentUserId = "user-123";
            const string otherUserId = "user-456";

            var department = new Department { Id = 1, Name = "Engineering" };

            var currentEmployee = new Employee
            {
                Id = 100,
                EmployeeNumber = "EMP10001",
                FirstName = "First Name",
                LastName = "Last Name",
                Email = "anyemail@email.com",
                HireDate = new DateOnly(2025, 1, 1),
                DepartmentId = 1,
                Department = department,
                UserId = currentUserId,
                IsActive = true
            };

            var otherEmployee = new Employee
            {
                Id = 300,
                EmployeeNumber = "EMP20002",
                FirstName = "Other First",
                LastName = "Other Last",
                Email = "anyotheremail@email.com",
                HireDate = new DateOnly(2024, 8, 15),
                DepartmentId = 1,
                Department = department,
                UserId = otherUserId,
                IsActive = true
            };

            var currentUserService = new Mock<ICurrentUserService>();
            currentUserService.Setup(service => service.UserId).Returns(currentUserId);

            var handler = new GetMyEmployeeBenefitsHandler(dbContext, currentUserService.Object);

            await dbContext.Employees.AddRangeAsync(currentEmployee, otherEmployee);
            await dbContext.SaveChangesAsync(CancellationToken.None);

            var result = await handler.Handle(new GetMyEmployeeBenefitsQuery(), CancellationToken.None);

            result.Id.Should().Be(currentEmployee.Id);
            result.FirstName.Should().Be(currentEmployee.FirstName);
            result.LastName.Should().Be(currentEmployee.LastName);

            result.Id.Should().NotBe(otherEmployee.Id);
        }
    }
}

using Benefits.Application.Exceptions;
using Benefits.Application.Features.Employees.Queries.ViewEmployeeBenefits;
using Benefits.Domain;
using FluentAssertions;
using HealthBenefitsPortal.UnitTests.Application.Common;
using Microsoft.EntityFrameworkCore;

namespace HealthBenefitsPortal.UnitTests.Application.Features.Employees.Queries.ViewEmployeeBenefits
{
    public sealed class GetEmployeeBenefitsHandlerTests
    {
        DbContextOptions<TestDbContext> _options;

        public GetEmployeeBenefitsHandlerTests()
        {
            _options = new DbContextOptionsBuilder<TestDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
        }

        [Fact]
        public async Task Handle_WhenActiveEmployeeHasEnrollments_ReturnsEmployeeAndBenefitDetails()
        {
            await using var dbContext = new TestDbContext(_options);

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
                IsActive = true
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

            await dbContext.Employees.AddAsync(employee, CancellationToken.None);
            await dbContext.SaveChangesAsync(CancellationToken.None);

            var handler = new GetEmployeeBenefitsHandler(dbContext);

            var query = new GetEmployeeBenefitsQuery(employee.Id);

            var result = await handler.Handle(query, CancellationToken.None);

            result.Should().NotBeNull();
            result.Id.Should().Be(employee.Id);
            result.FirstName.Should().Be(employee.FirstName);
            result.LastName.Should().Be(employee.LastName);
            result.Department.Should().Be(department.Name);

            result.Benefits.Should().ContainSingle();

            var returnedBenefit = result.Benefits.Single();

            returnedBenefit.BenefitPlanId.Should().Be(benefitPlan.Id);
            returnedBenefit.BenefitPlan.Should().Be(benefitPlan.Name);
            returnedBenefit.BenefitType.Should().Be(benefitType.Name);
            returnedBenefit.EnrollmentDate.Should().Be(enrollment.EnrollmentDate);
            returnedBenefit.EndDate.Should().BeNull();
            returnedBenefit.IsActive.Should().BeTrue();
        }

        [Fact]
        public async Task Handle_WhenEmployeeDoesNotExist_ThrowsNotFoundException()
        {
            await using var dbContext = new TestDbContext(_options);

            const int anyNonExistentEmployeeId = 9999;

            var handler = new GetEmployeeBenefitsHandler(dbContext);
            var query = new GetEmployeeBenefitsQuery(anyNonExistentEmployeeId);

            Func<Task> act = () => handler.Handle(query, CancellationToken.None);

            var exception = await act.Should().ThrowAsync<NotFoundException>();

            exception.Which.Message.Should().Contain(anyNonExistentEmployeeId.ToString());
        }

        [Fact]
        public async Task Handle_WhenEmployeeIsInactive_ThrowsNotFoundException()
        {
            await using var dbContext = new TestDbContext(_options);

            var department = new Department { Id = 1, Name = "Engineering" };

            var employee = new Employee
            {
                Id = 333,
                EmployeeNumber = "EMP12345",
                FirstName = "First Name",
                LastName = "Last Name",
                Email = "anyemail@email.com",
                HireDate = new DateOnly(2025, 1, 4),
                DepartmentId = department.Id,
                Department = department,
                IsActive = false
            };

            await dbContext.Employees.AddAsync(employee, CancellationToken.None);
            await dbContext.SaveChangesAsync(CancellationToken.None);

            var handler = new GetEmployeeBenefitsHandler(dbContext);
            var query = new GetEmployeeBenefitsQuery(employee.Id);

            Func<Task> act = () => handler.Handle(query, CancellationToken.None);

            var exception = await act.Should().ThrowAsync<NotFoundException>();

            exception.Which.Message.Should().Contain(employee.Id.ToString());
        }

        [Fact]
        public async Task Handle_WhenEmployeeHasTerminatedEnrolments_ReturnsEnrollmentHistory()
        {
            await using var dbContext = new TestDbContext(_options);

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
                IsActive = true
            };

            var enrollment = new EmployeeEnrollment
            {
                EmployeeId = employee.Id,
                BenefitPlanId = benefitPlan.Id,
                EnrollmentDate = new DateOnly(2025, 6, 6),
                Employee = employee,
                BenefitPlan = benefitPlan
            };

            enrollment.EndEnrollment(new DateOnly(2026, 5, 5));

            employee.Enrollments = new List<EmployeeEnrollment> { enrollment };

            await dbContext.Employees.AddAsync(employee, CancellationToken.None);
            await dbContext.SaveChangesAsync(CancellationToken.None);

            var handler = new GetEmployeeBenefitsHandler(dbContext);

            var result = await handler.Handle(new GetEmployeeBenefitsQuery(employee.Id), CancellationToken.None);

            result.Benefits.Should().ContainSingle();

            var returnedEnrollment = result.Benefits[0];

            returnedEnrollment.BenefitPlanId.Should().Be(benefitPlan.Id);
            returnedEnrollment.EnrollmentDate.Should().Be(new DateOnly(2025, 6, 6));
            returnedEnrollment.EndDate.Should().Be(new DateOnly(2026, 5, 5));
            returnedEnrollment.IsActive.Should().BeFalse();
        }
    }
}

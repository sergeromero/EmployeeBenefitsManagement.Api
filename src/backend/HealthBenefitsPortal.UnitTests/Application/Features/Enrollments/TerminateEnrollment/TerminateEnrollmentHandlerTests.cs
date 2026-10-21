using Benefits.Application.Exceptions;
using Benefits.Application.Exceptions.BusinessRuleViolationException;
using Benefits.Application.Features.Enrollments.TerminateEnrollment;
using Benefits.Application.Infrastructure.Contracts;
using Benefits.Domain;
using FluentAssertions;
using HealthBenefitsPortal.UnitTests.Application.Common;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace HealthBenefitsPortal.UnitTests.Application.Features.Enrollments.TerminateEnrollment
{
    public class TerminateEnrollmentHandlerTests
    {
        DbContextOptions<TestDbContext> _options;
        public TerminateEnrollmentHandlerTests()
        {
            _options = new DbContextOptionsBuilder<TestDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
        }

        [Fact]
        public async Task Handle_WhenEnrollmentIsActive_TerminatesEnrollment()
        {
            await using var dbContext = new TestDbContext(_options);

            const string currentUserId = "user-123";

            var enrollmentDate = new DateOnly(2026, 1, 1);
            var endDate = new DateOnly(2026, 10, 15);

            var department = new Department { Id = 1, Name = "Engineering" };
            var benefitType = new BenefitType { Id = 1, Name = "Health" };
            var enrollmentCategory = new EnrollmentCategory { Id = 1, Name = "Health Benefits" };

            var employee = new Employee
            {
                Id = 100,
                EmployeeNumber = "EMP10001",
                FirstName = "First Name",
                LastName = "Last Name",
                Email = "anyemail@email.com",
                HireDate = new DateOnly(2025, 1, 1),
                DepartmentId = department.Id,
                Department = department,
                UserId = currentUserId,
                IsActive = true
            };

            var benefitPlan = new BenefitPlan
            {
                Id = 10,
                Name = "Extended Health",
                Description = "Extended health coverage",
                BenefitTypeId = benefitType.Id,
                BenefitType = benefitType,
                EnrollmentCategoryId = enrollmentCategory.Id,
                EnrollmentCategory = enrollmentCategory
            };

            var enrollment = new EmployeeEnrollment
            {
                EmployeeId = employee.Id,
                BenefitPlanId = benefitPlan.Id,
                EnrollmentDate = enrollmentDate,
                Employee = employee,
                BenefitPlan = benefitPlan
            };

            employee.Enrollments = new List<EmployeeEnrollment>
            {
                enrollment
            };

            await dbContext.Employees.AddAsync(employee, CancellationToken.None);
            await dbContext.BenefitPlans.AddAsync(benefitPlan, CancellationToken.None);
            await dbContext.EmployeeEnrollments.AddAsync(enrollment, CancellationToken.None);
            await dbContext.SaveChangesAsync(CancellationToken.None);

            var currentUserService = new Mock<ICurrentUserService>();

            currentUserService.Setup(service => service.UserId).Returns(currentUserId);

            var handler = new TerminateEnrollmentHandler(dbContext, currentUserService.Object);

            var command = new TerminateEnrollmentCommand(BenefitPlanId: benefitPlan.Id, EndDate: endDate);

            await handler.Handle(command, CancellationToken.None);

            var persistedEnrollment = await dbContext.EmployeeEnrollments.SingleAsync(CancellationToken.None);

            persistedEnrollment.EmployeeId.Should().Be(employee.Id);
            persistedEnrollment.BenefitPlanId.Should().Be(benefitPlan.Id);
            persistedEnrollment.EnrollmentDate.Should().Be(enrollmentDate);
            persistedEnrollment.EndDate.Should().Be(endDate);
            persistedEnrollment.EndDate.Should().NotBeNull();

            (await dbContext.EmployeeEnrollments.CountAsync(CancellationToken.None)).Should().Be(1);

            currentUserService.VerifyGet(service => service.UserId, Times.Once);
        }

        [Fact]
        public async Task Handle_WhenEnrollmentDoesNotExist_ThrowsNotFoundException()
        {
            await using var dbContext = new TestDbContext(_options);

            const string currentUserId = "user-123";
            const int benefitPlanId = 10;

            var department = new Department { Id = 1, Name = "Engineering" };

            var employee = new Employee
            {
                Id = 100,
                EmployeeNumber = "EMP10001",
                FirstName = "First Name",
                LastName = "Last Name",
                Email = "anyemail@email.com",
                HireDate = new DateOnly(2025, 1, 1),
                DepartmentId = department.Id,
                Department = department,
                UserId = currentUserId,
                IsActive = true
            };

            await dbContext.Employees.AddAsync(employee, CancellationToken.None);
            await dbContext.SaveChangesAsync(CancellationToken.None);

            var currentUserService = new Mock<ICurrentUserService>();

            currentUserService.Setup(service => service.UserId).Returns(currentUserId);

            var handler = new TerminateEnrollmentHandler(dbContext, currentUserService.Object);

            var command = new TerminateEnrollmentCommand(BenefitPlanId: benefitPlanId, EndDate: new DateOnly(2026, 10, 15));

            var act = () => handler.Handle(command, CancellationToken.None);

            await act.Should()
                .ThrowAsync<NotFoundException>()
                .WithMessage($"No enrollment found with Employee Id {employee.Id} and Benefit Plan Id {benefitPlanId}.");

            (await dbContext.EmployeeEnrollments.CountAsync(CancellationToken.None)).Should().Be(0);
        }

        [Fact]
        public async Task Handle_WhenEnrollmentIsAlreadyTerminated_ThrowsEnrollmentAlreadyTerminatedException()
        {
            await using var dbContext = new TestDbContext(_options);

            const string currentUserId = "user-123";

            var enrollmentDate = new DateOnly(2026, 1, 1);
            var originalEndDate = new DateOnly(2026, 6, 30);
            var requestedEndDate = new DateOnly(2026, 10, 15);

            var department = new Department { Id = 1, Name = "Engineering" };
            var benefitType = new BenefitType { Id = 1, Name = "Health" };
            var enrollmentCategory = new EnrollmentCategory { Id = 1, Name = "Health Benefits" };

            var employee = new Employee
            {
                Id = 100,
                EmployeeNumber = "EMP10001",
                FirstName = "First Name",
                LastName = "Last Name",
                Email = "anyemail@email.com",
                HireDate = new DateOnly(2025, 1, 1),
                DepartmentId = department.Id,
                Department = department,
                UserId = currentUserId,
                IsActive = true
            };

            var benefitPlan = new BenefitPlan
            {
                Id = 10,
                Name = "Extended Health",
                Description = "Extended health coverage",
                BenefitTypeId = benefitType.Id,
                BenefitType = benefitType,
                EnrollmentCategoryId = enrollmentCategory.Id,
                EnrollmentCategory = enrollmentCategory
            };

            var enrollment = new EmployeeEnrollment
            {
                EmployeeId = employee.Id,
                BenefitPlanId = benefitPlan.Id,
                EnrollmentDate = enrollmentDate,
                Employee = employee,
                BenefitPlan = benefitPlan
            };

            enrollment.EndEnrollment(originalEndDate);

            employee.Enrollments = new List<EmployeeEnrollment>
            {
                enrollment
            };

            await dbContext.Employees.AddAsync(employee, CancellationToken.None);
            await dbContext.BenefitPlans.AddAsync(benefitPlan, CancellationToken.None);
            await dbContext.EmployeeEnrollments.AddAsync(enrollment, CancellationToken.None);
            await dbContext.SaveChangesAsync(CancellationToken.None);

            var currentUserService = new Mock<ICurrentUserService>();

            currentUserService.Setup(service => service.UserId).Returns(currentUserId);

            var handler = new TerminateEnrollmentHandler(dbContext, currentUserService.Object);

            var command = new TerminateEnrollmentCommand(BenefitPlanId: benefitPlan.Id,  EndDate: requestedEndDate);

            var act = () => handler.Handle(command, CancellationToken.None);

            await act.Should().ThrowAsync<EnrollmentAlreadyTerminatedException>();

            var persistedEnrollment = await dbContext.EmployeeEnrollments.SingleAsync(CancellationToken.None);

            persistedEnrollment.EndDate.Should().Be(originalEndDate);
            persistedEnrollment.EndDate.Should().NotBeNull();
        }

        [Fact]
        public async Task Handle_WhenEndDatePrecedesEnrollmentDate_ThrowsInvalidEnrollmentDateException()
        {
            await using var dbContext = new TestDbContext(_options);

            const string currentUserId = "user-123";

            var enrollmentDate = new DateOnly(2026, 6, 15);
            var requestedEndDate = new DateOnly(2026, 6, 14);

            var department = new Department { Id = 1, Name = "Engineering" };
            var benefitType = new BenefitType { Id = 1, Name = "Health" };
            var enrollmentCategory = new EnrollmentCategory { Id = 1, Name = "Health Benefits" };

            var employee = new Employee
            {
                Id = 100,
                EmployeeNumber = "EMP10001",
                FirstName = "First Name",
                LastName = "Last Name",
                Email = "anyemail@email.com",
                HireDate = new DateOnly(2025, 1, 1),
                DepartmentId = department.Id,
                Department = department,
                UserId = currentUserId,
                IsActive = true
            };

            var benefitPlan = new BenefitPlan
            {
                Id = 10,
                Name = "Extended Health",
                Description = "Extended health coverage",
                BenefitTypeId = benefitType.Id,
                BenefitType = benefitType,
                EnrollmentCategoryId = enrollmentCategory.Id,
                EnrollmentCategory = enrollmentCategory
            };

            var enrollment = new EmployeeEnrollment
            {
                EmployeeId = employee.Id,
                BenefitPlanId = benefitPlan.Id,
                EnrollmentDate = enrollmentDate,
                Employee = employee,
                BenefitPlan = benefitPlan
            };

            employee.Enrollments = new List<EmployeeEnrollment>
            {
                enrollment
            };

            await dbContext.Employees.AddAsync(employee, CancellationToken.None);
            await dbContext.BenefitPlans.AddAsync(benefitPlan, CancellationToken.None);
            await dbContext.EmployeeEnrollments.AddAsync(enrollment, CancellationToken.None);
            await dbContext.SaveChangesAsync(CancellationToken.None);

            var currentUserService = new Mock<ICurrentUserService>();

            currentUserService.Setup(service => service.UserId).Returns(currentUserId);

            var handler = new TerminateEnrollmentHandler(dbContext, currentUserService.Object);

            var command = new TerminateEnrollmentCommand(BenefitPlanId: benefitPlan.Id, EndDate: requestedEndDate);

            var act = () => handler.Handle(command, CancellationToken.None);

            await act.Should()
                .ThrowAsync<InvalidEnrollmentDateException>()
                .WithMessage("An enrollment cannot end before it started.");

            var persistedEnrollment = await dbContext.EmployeeEnrollments.SingleAsync(CancellationToken.None);

            persistedEnrollment.EndDate.Should().BeNull();
            persistedEnrollment.EndDate.Should().BeNull();
        }

        [Fact]
        public async Task Handle_WhenCurrentUserIdIsMissing_ThrowsNotFoundException()
        {
            await using var dbContext = new TestDbContext(_options);

            var currentUserService = new Mock<ICurrentUserService>();

            currentUserService.Setup(service => service.UserId).Returns((string?)null);

            var handler = new TerminateEnrollmentHandler(dbContext, currentUserService.Object);

            var command = new TerminateEnrollmentCommand(BenefitPlanId: 10, EndDate: new DateOnly(2026, 10, 15));

            var act = () => handler.Handle(command, CancellationToken.None);

            await act.Should()
                .ThrowAsync<NotFoundException>()
                .WithMessage("No active employee record is linked to the authenticated user.");

            currentUserService.VerifyGet(service => service.UserId, Times.Once);

            (await dbContext.EmployeeEnrollments.CountAsync(CancellationToken.None)).Should().Be(0);
        }

        [Fact]
        public async Task Handle_WhenCurrentEmployeeIsInactive_ThrowsNotFoundException()
        {
            await using var dbContext = new TestDbContext(_options);

            const string currentUserId = "user-123";

            var employee = new Employee
            {
                Id = 100,
                EmployeeNumber = "EMP10001",
                FirstName = "First Name",
                LastName = "Last Name",
                Email = "anyemail@email.com",
                HireDate = new DateOnly(2025, 1, 1),
                DepartmentId = 1,
                Department = new Department {  Id = 1, Name = "Engineering" },
                UserId = currentUserId,
                IsActive = false
            };

            await dbContext.Employees.AddAsync(employee, CancellationToken.None);
            await dbContext.SaveChangesAsync(CancellationToken.None);

            var currentUserService = new Mock<ICurrentUserService>();

            currentUserService.Setup(service => service.UserId).Returns(currentUserId);

            var handler = new TerminateEnrollmentHandler(dbContext, currentUserService.Object);

            var command = new TerminateEnrollmentCommand(BenefitPlanId: 10, EndDate: new DateOnly(2026, 10, 15));

            var act = () => handler.Handle(command, CancellationToken.None);

            await act.Should().ThrowAsync<NotFoundException>()
                .WithMessage("No active employee record is linked to the authenticated user.");

            (await dbContext.EmployeeEnrollments.CountAsync(CancellationToken.None)).Should().Be(0);
        }

        [Fact]
        public async Task Handle_WhenCurrentUserHasNoLinkedEmployee_ThrowsNotFoundException()
        {
            await using var dbContext = new TestDbContext(_options);

            const string currentUserId = "user-123";

            var currentUserService = new Mock<ICurrentUserService>();

            currentUserService.Setup(service => service.UserId).Returns(currentUserId);

            var handler = new TerminateEnrollmentHandler(dbContext, currentUserService.Object);

            var command = new TerminateEnrollmentCommand(BenefitPlanId: 10, EndDate: new DateOnly(2026, 10, 16));

            var act = () => handler.Handle(command, CancellationToken.None);

            await act.Should()
                .ThrowAsync<NotFoundException>()
                .WithMessage("No active employee record is linked to the authenticated user.");

            currentUserService.VerifyGet(service => service.UserId, Times.Once);

            (await dbContext.EmployeeEnrollments.CountAsync(CancellationToken.None)).Should().Be(0);
        }

        [Fact]
        public async Task Handle_WhenEnrollmentBelongsToAnotherEmployee_ThrowsNotFoundException()
        {
            await using var dbContext = new TestDbContext(_options);

            const string currentUserId = "user-123";
            var department = new Department { Id = 1, Name = "Engineering" };

            var currentEmployee = new Employee
            {
                Id = 1,
                EmployeeNumber = "EMP10002",
                FirstName = "First Name",
                LastName = "Last Name",
                Email = "anyemail@email.com",
                UserId = currentUserId,
                DepartmentId = department.Id,
                Department = department,
                IsActive = true
            };

            var anotherEmployee = new Employee
            {
                Id = 2,
                UserId = "another-user-456",
                EmployeeNumber = "EMP10005",
                FirstName = "Other First",
                LastName = "Other Last",
                Email = "anyotheremail@email.com",
                DepartmentId = department.Id,
                Department = department,
                IsActive = true
            };

            var enrollment = new EmployeeEnrollment
            {
                EmployeeId = anotherEmployee.Id,
                BenefitPlanId = 10,
                EnrollmentDate = new DateOnly(2026, 1, 1)
            };

            dbContext.Employees.AddRange(currentEmployee, anotherEmployee);
            dbContext.EmployeeEnrollments.Add(enrollment);
            await dbContext.SaveChangesAsync(CancellationToken.None);

            var currentUserService = new Mock<ICurrentUserService>();
            currentUserService.Setup(service => service.UserId).Returns(currentUserId);

            var handler = new TerminateEnrollmentHandler(dbContext, currentUserService.Object);

            var command = new TerminateEnrollmentCommand(BenefitPlanId: 10, EndDate: new DateOnly(2026, 10, 16));

            var act = () => handler.Handle(command, CancellationToken.None);

            await act.Should().ThrowAsync<NotFoundException>()
                .WithMessage($"No enrollment found with Employee Id {currentEmployee.Id} and Benefit Plan Id {command.BenefitPlanId}.");

            enrollment.EndDate.Should().BeNull();
            enrollment.EndDate.Should().BeNull();

            currentUserService.VerifyGet(service => service.UserId, Times.Once);
        }
    }
}

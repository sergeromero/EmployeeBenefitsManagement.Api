using Benefits.Application.Exceptions;
using Benefits.Application.Exceptions.BusinessRuleViolationException;
using Benefits.Application.Features.Enrollments.CreateEnrollment;
using Benefits.Application.Infrastructure.Contracts;
using Benefits.Domain;
using FluentAssertions;
using HealthBenefitsPortal.UnitTests.Application.Common;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace HealthBenefitsPortal.UnitTests.Application.Features.Enrollments.CreateEnrollment
{
    public class CreateEnrollmentHandlerTests
    {
        DbContextOptions<TestDbContext> _options;

        public CreateEnrollmentHandlerTests()
        {
            _options = new DbContextOptionsBuilder<TestDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
        }

        [Fact]
        public async Task Handle_WhenRequestIsValid_CreatesEnrollmentForCurrentEmployee()
        {
            await using var dbContext = new TestDbContext(_options);

            const string currentUserId = "user-123";

            var today = DateOnly.FromDateTime(
                TimeProvider.System.GetUtcNow().UtcDateTime);

            var department = new Department { Id = 1, Name = "Engineering" };
            var benefitType = new BenefitType { Id = 1, Name = "Health" };
            var enrollmentCategory = new EnrollmentCategory { Id = 1, Name = "Health Benefits" };

            var benefitPlan = new BenefitPlan
            {
                Id = 10,
                Name = "Extended Health",
                Description = "Extended health coverage",
                BenefitTypeId = benefitType.Id,
                BenefitType = benefitType,
                EnrollmentCategoryId = enrollmentCategory.Id,
                EnrollmentCategory = enrollmentCategory,
                IsActive = true
            };

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
            await dbContext.BenefitPlans.AddAsync(benefitPlan, CancellationToken.None);
            await dbContext.SaveChangesAsync(CancellationToken.None);

            var currentUserService = new Mock<ICurrentUserService>();

            currentUserService.Setup(service => service.UserId).Returns(currentUserId);

            var handler = new CreateEnrollmentHandler(dbContext, currentUserService.Object, TimeProvider.System);

            var command = new CreateEnrollmentCommand(BenefitPlanId: benefitPlan.Id, EnrollmentDate: today);

            var result = await handler.Handle(command, CancellationToken.None);

            result.Should().Be(MediatR.Unit.Value);

            var enrollment = await dbContext.EmployeeEnrollments.SingleAsync(CancellationToken.None);

            enrollment.EmployeeId.Should().Be(employee.Id);
            enrollment.BenefitPlanId.Should().Be(benefitPlan.Id);
            enrollment.EnrollmentDate.Should().Be(today);
            enrollment.EndDate.Should().BeNull();

            currentUserService.VerifyGet(service => service.UserId, Times.Once);
        }

        [Fact]
        public async Task Handle_WhenEmployeeHasActiveEnrollmentInSameCategory_ThrowsBusinessRuleException()
        {
            await using var dbContext = new TestDbContext(_options);

            const string currentUserId = "user-123";

            var today = DateOnly.FromDateTime(TimeProvider.System.GetUtcNow().UtcDateTime);
            var department = new Department { Id = 1, Name = "Engineering" };
            var benefitType = new BenefitType { Id = 1, Name = "Health" };
            var enrollmentCategory = new EnrollmentCategory { Id = 1, Name = "Health Benefits" };

            var existingPlan = new BenefitPlan
            {
                Id = 10,
                Name = "Basic Health",
                Description = "Basic health coverage",
                BenefitTypeId = benefitType.Id,
                BenefitType = benefitType,
                EnrollmentCategoryId = enrollmentCategory.Id,
                EnrollmentCategory = enrollmentCategory
            };

            var requestedPlan = new BenefitPlan
            {
                Id = 20,
                Name = "Extended Health",
                Description = "Extended health coverage",
                BenefitTypeId = benefitType.Id,
                BenefitType = benefitType,
                EnrollmentCategoryId = enrollmentCategory.Id,
                EnrollmentCategory = enrollmentCategory
            };

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

            var existingEnrollment = new EmployeeEnrollment
            {
                EmployeeId = employee.Id,
                BenefitPlanId = existingPlan.Id,
                EnrollmentDate = today,
                Employee = employee,
                BenefitPlan = existingPlan
            };

            employee.Enrollments = new List<EmployeeEnrollment>
            {
                existingEnrollment
            };

            await dbContext.Employees.AddAsync(employee, CancellationToken.None);
            await dbContext.BenefitPlans.AddAsync(requestedPlan, CancellationToken.None);
            await dbContext.EmployeeEnrollments.AddAsync(existingEnrollment, CancellationToken.None);
            await dbContext.SaveChangesAsync(CancellationToken.None);

            var currentUserService = new Mock<ICurrentUserService>();

            currentUserService.Setup(service => service.UserId).Returns(currentUserId);

            var handler = new CreateEnrollmentHandler(dbContext, currentUserService.Object, TimeProvider.System);

            var command = new CreateEnrollmentCommand(BenefitPlanId: requestedPlan.Id, EnrollmentDate: today);

            var act = () => handler.Handle(command, CancellationToken.None);

            await act.Should().ThrowAsync<BusinessRuleException>();

            (await dbContext.EmployeeEnrollments.CountAsync(CancellationToken.None)).Should().Be(1);
        }

        [Fact]
        public async Task Handle_WhenPreviousEnrollmentInSameCategoryIsTerminated_CreatesEnrollment()
        {
            await using var dbContext = new TestDbContext(_options);

            const string currentUserId = "user-123";

            var today = DateOnly.FromDateTime(TimeProvider.System.GetUtcNow().UtcDateTime); 
            var department = new Department { Id = 1, Name = "Engineering" };
            var benefitType = new BenefitType { Id = 1, Name = "Health" };
            var enrollmentCategory = new EnrollmentCategory { Id = 1, Name = "Health Benefits" };

            var previousPlan = new BenefitPlan
            {
                Id = 10,
                Name = "Basic Health",
                Description = "Basic health coverage",
                BenefitTypeId = benefitType.Id,
                BenefitType = benefitType,
                EnrollmentCategoryId = enrollmentCategory.Id,
                EnrollmentCategory = enrollmentCategory
            };

            var requestedPlan = new BenefitPlan
            {
                Id = 20,
                Name = "Extended Health",
                Description = "Extended health coverage",
                BenefitTypeId = benefitType.Id,
                BenefitType = benefitType,
                EnrollmentCategoryId = enrollmentCategory.Id,
                EnrollmentCategory = enrollmentCategory
            };

            var employee = new Employee
            {
                Id = 100,
                EmployeeNumber = "EMP100",
                FirstName = "Alex",
                LastName = "Morgan",
                Email = "alex.morgan@example.com",
                HireDate = new DateOnly(2025, 1, 1),
                DepartmentId = department.Id,
                Department = department,
                UserId = currentUserId,
                IsActive = true
            };

            var previousEnrollment = new EmployeeEnrollment
            {
                EmployeeId = employee.Id,
                BenefitPlanId = previousPlan.Id,
                EnrollmentDate = today.AddMonths(-3),
                Employee = employee,
                BenefitPlan = previousPlan
            };

            previousEnrollment.EndEnrollment(today.AddDays(-1));

            employee.Enrollments = new List<EmployeeEnrollment>
            {
                previousEnrollment
            };

            await dbContext.Employees.AddAsync(employee, CancellationToken.None);
            await dbContext.BenefitPlans.AddRangeAsync(previousPlan, requestedPlan);
            await dbContext.EmployeeEnrollments.AddAsync(previousEnrollment, CancellationToken.None);
            await dbContext.SaveChangesAsync(CancellationToken.None);

            var currentUserService = new Mock<ICurrentUserService>();

            currentUserService.Setup(service => service.UserId).Returns(currentUserId);

            var handler = new CreateEnrollmentHandler(dbContext, currentUserService.Object, TimeProvider.System);

            var command = new CreateEnrollmentCommand(BenefitPlanId: requestedPlan.Id, EnrollmentDate: today);

            await handler.Handle(command, CancellationToken.None);

            var enrollments = await dbContext.EmployeeEnrollments.OrderBy(enrollment => enrollment.BenefitPlanId).ToListAsync(CancellationToken.None);

            enrollments.Should().HaveCount(2);

            var newEnrollment = enrollments.Single(enrollment => enrollment.BenefitPlanId == requestedPlan.Id);

            newEnrollment.EmployeeId.Should().Be(employee.Id);
            newEnrollment.EnrollmentDate.Should().Be(today);
            newEnrollment.EndDate.Should().BeNull();

            var historicalEnrollment = enrollments.Single(enrollment => enrollment.BenefitPlanId == previousPlan.Id);

            historicalEnrollment.EndDate.Should().Be(today.AddDays(-1));
            historicalEnrollment.EndDate.Should().NotBeNull();
        }

        [Fact]
        public async Task Handle_WhenCurrentEmployeeIsInactive_ThrowsNotFoundException()
        {
            await using var dbContext = new TestDbContext(_options);

            const string currentUserId = "user-123";
            var department = new Department { Id = 1, Name = "Engineering" };
            var benefitType = new BenefitType { Id = 1, Name = "Health" };
            var enrollmentCategory = new EnrollmentCategory { Id = 1, Name = "Health Benefits" };

            var benefitPlan = new BenefitPlan
            {
                Id = 10,
                Name = "Extended Health",
                Description = "Extended health coverage",
                BenefitTypeId = benefitType.Id,
                BenefitType = benefitType,
                EnrollmentCategoryId = enrollmentCategory.Id,
                EnrollmentCategory = enrollmentCategory,
                IsActive = true
            };

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
                IsActive = false
            };

            await dbContext.Employees.AddAsync(employee, CancellationToken.None);
            await dbContext.BenefitPlans.AddAsync(benefitPlan, CancellationToken.None);
            await dbContext.SaveChangesAsync(CancellationToken.None);

            var currentUserService = new Mock<ICurrentUserService>();

            currentUserService.Setup(service => service.UserId).Returns(currentUserId);

            var handler = new CreateEnrollmentHandler(dbContext, currentUserService.Object,
                TimeProvider.System);

            var command = new CreateEnrollmentCommand(
                BenefitPlanId: benefitPlan.Id,
                EnrollmentDate: DateOnly.FromDateTime(
                    TimeProvider.System.GetUtcNow().UtcDateTime));

            var act = () => handler.Handle(
                command,
                CancellationToken.None);

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

            var handler = new CreateEnrollmentHandler(dbContext, currentUserService.Object, TimeProvider.System);

            var command = new CreateEnrollmentCommand(BenefitPlanId: 10,  EnrollmentDate: DateOnly.FromDateTime(TimeProvider.System.GetUtcNow().UtcDateTime));

            var act = () => handler.Handle(command, CancellationToken.None);

            await act.Should()
                .ThrowAsync<NotFoundException>()
                .WithMessage("No active employee record is linked to the authenticated user.");

            (await dbContext.EmployeeEnrollments.CountAsync(CancellationToken.None)).Should().Be(0);

            currentUserService.VerifyGet(service => service.UserId, Times.Once);
        }

        [Fact]
        public async Task Handle_WhenBenefitPlanDoesNotExist_ThrowsNotFoundException()
        {
            await using var dbContext = new TestDbContext(_options);

            const string currentUserId = "user-123";
            const int benefitPlanId = 999;

            var employee = new Employee
            {
                Id = 100,
                EmployeeNumber = "EMP10001",
                FirstName = "First Name",
                LastName = "Last Name",
                Email = "anyemail@email.com",
                HireDate = new DateOnly(2025, 1, 1),
                DepartmentId = 1,
                Department = new Department { Id = 1, Name = "Engineerign" },
                UserId = currentUserId,
                IsActive = true
            };

            await dbContext.Employees.AddAsync(employee, CancellationToken.None);
            await dbContext.SaveChangesAsync(CancellationToken.None);

            var currentUserService = new Mock<ICurrentUserService>();

            currentUserService.Setup(service => service.UserId).Returns(currentUserId);

            var handler = new CreateEnrollmentHandler(dbContext, currentUserService.Object, TimeProvider.System);

            var command = new CreateEnrollmentCommand(BenefitPlanId: benefitPlanId, EnrollmentDate: DateOnly.FromDateTime(TimeProvider.System.GetUtcNow().UtcDateTime));

            var act = () => handler.Handle(command, CancellationToken.None);

            await act.Should().ThrowAsync<NotFoundException>()
                .WithMessage($"Benefit Plan with id {benefitPlanId} was not found.");

            (await dbContext.EmployeeEnrollments.CountAsync(CancellationToken.None)).Should().Be(0);
        }

        [Fact]
        public async Task Handle_WhenBenefitPlanIsInactive_ThrowsNotFoundException()
        {
            await using var dbContext = new TestDbContext(_options);

            const string currentUserId = "user-123";
            var department = new Department { Id = 1, Name = "Engineering" };
            var benefitType = new BenefitType { Id = 1, Name = "Health" };
            var enrollmentCategory = new EnrollmentCategory { Id = 1, Name = "Health Benefits" };

            var inactiveBenefitPlan = new BenefitPlan
            {
                Id = 10,
                Name = "Extended Health",
                Description = "Extended health coverage",
                BenefitTypeId = benefitType.Id,
                BenefitType = benefitType,
                EnrollmentCategoryId = enrollmentCategory.Id,
                EnrollmentCategory = enrollmentCategory,
                IsActive = false
            };

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
            await dbContext.BenefitPlans.AddAsync(inactiveBenefitPlan, CancellationToken.None);
            await dbContext.SaveChangesAsync(CancellationToken.None);

            var currentUserService = new Mock<ICurrentUserService>();

            currentUserService.Setup(service => service.UserId).Returns(currentUserId);

            var handler = new CreateEnrollmentHandler(dbContext, currentUserService.Object, TimeProvider.System);

            var command = new CreateEnrollmentCommand(BenefitPlanId: inactiveBenefitPlan.Id, EnrollmentDate: DateOnly.FromDateTime(TimeProvider.System.GetUtcNow().UtcDateTime));

            var act = () => handler.Handle(command, CancellationToken.None);

            await act.Should().ThrowAsync<NotFoundException>()
                .WithMessage($"Benefit Plan with id {inactiveBenefitPlan.Id} was not found.");

            (await dbContext.EmployeeEnrollments.CountAsync(CancellationToken.None)).Should().Be(0);
        }

        [Fact]
        public async Task Handle_WhenEnrollmentDateIsInThePast_ThrowsBusinessRuleException()
        {
            await using var dbContext = new TestDbContext(_options);

            const string currentUserId = "user-123";

            var today = DateOnly.FromDateTime(TimeProvider.System.GetUtcNow().UtcDateTime);
            var department = new Department { Id = 1, Name = "Engineering" };
            var benefitType = new BenefitType { Id = 1, Name = "Health" };
            var enrollmentCategory = new EnrollmentCategory { Id = 1, Name = "Health Benefits" };

            var benefitPlan = new BenefitPlan
            {
                Id = 10,
                Name = "Extended Health",
                Description = "Extended health coverage",
                BenefitTypeId = benefitType.Id,
                BenefitType = benefitType,
                EnrollmentCategoryId = enrollmentCategory.Id,
                EnrollmentCategory = enrollmentCategory,
                IsActive = true
            };

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
            await dbContext.BenefitPlans.AddAsync(benefitPlan, CancellationToken.None);
            await dbContext.SaveChangesAsync(CancellationToken.None);

            var currentUserService = new Mock<ICurrentUserService>();

            currentUserService.Setup(service => service.UserId).Returns(currentUserId);

            var handler = new CreateEnrollmentHandler(dbContext, currentUserService.Object, TimeProvider.System);

            var command = new CreateEnrollmentCommand(BenefitPlanId: benefitPlan.Id, EnrollmentDate: today.AddDays(-1));

            var act = () => handler.Handle(command, CancellationToken.None);

            await act.Should().ThrowAsync<BusinessRuleException>()
                .WithMessage("Enrollment date cannot be in the past.");

            (await dbContext.EmployeeEnrollments.CountAsync(CancellationToken.None)).Should().Be(0);
        }

        [Fact]
        public async Task Handle_WhenEnrollmentDateIsInTheFuture_CreatesEnrollment()
        {
            await using var dbContext = new TestDbContext(_options);

            const string currentUserId = "user-123";

            var today = DateOnly.FromDateTime(TimeProvider.System.GetUtcNow().UtcDateTime);
            var department = new Department { Id = 1, Name = "Engineering" };
            var benefitType = new BenefitType { Id = 1, Name = "Health" };
            var enrollmentCategory = new EnrollmentCategory { Id = 1, Name = "Health Benefits" };

            var benefitPlan = new BenefitPlan
            {
                Id = 10,
                Name = "Extended Health",
                Description = "Extended health coverage",
                BenefitTypeId = benefitType.Id,
                BenefitType = benefitType,
                EnrollmentCategoryId = enrollmentCategory.Id,
                EnrollmentCategory = enrollmentCategory,
                IsActive = true
            };

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
            await dbContext.BenefitPlans.AddAsync(benefitPlan, CancellationToken.None);
            await dbContext.SaveChangesAsync(CancellationToken.None);

            var currentUserService = new Mock<ICurrentUserService>();

            currentUserService.Setup(service => service.UserId).Returns(currentUserId);

            var handler = new CreateEnrollmentHandler( dbContext, currentUserService.Object, TimeProvider.System);

            var futureDate = today.AddDays(30);

            var command = new CreateEnrollmentCommand(BenefitPlanId: benefitPlan.Id, EnrollmentDate: futureDate);

            await handler.Handle(command, CancellationToken.None);

            var enrollment = await dbContext.EmployeeEnrollments.SingleAsync(CancellationToken.None);

            enrollment.EmployeeId.Should().Be(employee.Id);
            enrollment.BenefitPlanId.Should().Be(benefitPlan.Id);
            enrollment.EnrollmentDate.Should().Be(futureDate);
            enrollment.EndDate.Should().BeNull();
        }
    }
}

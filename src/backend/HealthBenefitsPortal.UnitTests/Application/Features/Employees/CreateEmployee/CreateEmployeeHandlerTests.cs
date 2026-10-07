using Benefits.Application.Features.Employees.CreateEmployee;
using Benefits.Application.Infrastructure.Contracts;
using Benefits.Domain;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace HealthBenefitsPortal.UnitTests.Application.Features.Employees.CreateEmployee
{
    public sealed class CreateEmployeeHandlerTests
    {
        [Fact]
        public async Task Handle_ValidCommand_AddsEmployeeAndSavesChanges()
        {
            var employees = new List<Employee>();
            var employeeDBSet = new Mock<DbSet<Employee>>();

            employeeDBSet.Setup(x => x.Add(It.IsAny<Employee>()))
                .Callback<Employee>(e => employees.Add(e));

            var dbContext = new Mock<IBenefitsDbContext>();

            dbContext.SetupGet(x => x.Employees)
                .Returns(employeeDBSet.Object);

            var handler = new CreateEmployeeHandler(dbContext.Object);

            var command = GetValidCommand();

            await handler.Handle(command, CancellationToken.None);

            employees.Should().ContainSingle();

            var employee = employees.Single();

            employee.EmployeeNumber.Should().Be(command.EmployeeNumber);
            employee.FirstName.Should().Be(command.FirstName);
            employee.LastName.Should().Be(command.LastName);
            employee.Email.Should().Be(command.Email);
            employee.HireDate.Should().Be(command.HireDate);
            employee.DepartmentId.Should().Be(command.DepartmentId);

            employeeDBSet.Verify(x => x.Add(It.IsAny<Employee>()), Times.Once);

            dbContext.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_WhenSaveChangesFails_PropagatesException()
        {
            const string message = "Database failure";

            var employeeDBSet = new Mock<DbSet<Employee>>();
            var dbContext = new Mock<IBenefitsDbContext>();

            dbContext.SetupGet(x => x.Employees)
                .Returns(employeeDBSet.Object);

            dbContext.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ThrowsAsync(new InvalidOperationException(message));

            var handler = new CreateEmployeeHandler(dbContext.Object);

            var command = GetValidCommand();

            Func<Task> act = () => handler.Handle(command, CancellationToken.None);

            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage(message);
        }

        [Fact]
        public async Task Handle_ValidCommand_PassesCancellationTokenToSaveChanges()
        {
            var employeeDbSet = new Mock<DbSet<Employee>>();
            var dbContext = new Mock<IBenefitsDbContext>();

            dbContext
                .SetupGet(x => x.Employees)
                .Returns(employeeDbSet.Object);

            var cancellationToken = new CancellationToken();

            var handler = new CreateEmployeeHandler(dbContext.Object);

            var command = GetValidCommand();

            await handler.Handle(command, cancellationToken);

            dbContext.Verify(x => x.SaveChangesAsync(cancellationToken),
                Times.Once);
        }

        private static CreateEmployeeCommand GetValidCommand()
        {
            return new CreateEmployeeCommand(
                EmployeeNumber: "EMP12345",
                FirstName: "FirstName",
                LastName: "LastName",
                Email: "email@something.com",
                HireDate: new DateOnly(2026, 5, 1),
                DepartmentId: 1);
        }
    }
}

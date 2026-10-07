using Benefits.Application.Features.Employees.CreateEmployee;
using FluentValidation.TestHelper;

namespace HealthBenefitsPortal.UnitTests.Application.Features.Employees.CreateEmployee
{
    public sealed class CreateEmployeeValidatorTests
    {
        private readonly CreateEmployeeValidator _validator = new();

        [Fact]
        public void Validate_ValidCommand_HasNoValidationErrors()
        {
            var command = CreateValidCommand();

            var result = _validator.TestValidate(command);

            result.ShouldNotHaveAnyValidationErrors();
        }

        #region EmployeeNumber
        [Fact]
        public void Validate_EmptyEmployeeNumber_HasNotEmptyError()
        {
            var command = CreateValidCommand() with
            {
                EmployeeNumber = string.Empty
            };

            var result = _validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x =>  x.EmployeeNumber).WithErrorCode("NotEmptyValidator");
        }

        [Theory]
        [InlineData("1")]
        [InlineData("12")]
        [InlineData("123")]
        [InlineData("1234")]
        [InlineData("12345")]
        [InlineData("123456")]
        [InlineData("1234567")]
        public void Validate_EmployeeNumberWithLessThanEightCharacters_HasMinimumLengthError(string employeeNumber)
        {
            var command = CreateValidCommand() with
            {
                EmployeeNumber = employeeNumber
            };

            var result = _validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.EmployeeNumber).WithErrorCode("MinimumLengthValidator");
        }

        [Fact]
        public void Validate_EmployeeNumberWithExactlyEightCharacters_HasNoValidationError()
        {
            var command = CreateValidCommand() with
            {
                EmployeeNumber = "12345678"
            };

            var result = _validator.TestValidate(command);

            result.ShouldNotHaveValidationErrorFor(x => x.EmployeeNumber);
        }

        [Fact]
        public void Validate_EmployeeNumberLargerThanEightCharacters_HasMaximumLengthError()
        {
            var command = CreateValidCommand() with
            {
                EmployeeNumber = "123456789"
            };

            var result = _validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.EmployeeNumber).WithErrorCode("MaximumLengthValidator");
        }
        #endregion

        #region FirstName
        [Fact]
        public void Validate_EmptyFirstName_HasNotEmptyError()
        {
            var command = CreateValidCommand() with
            {
                FirstName = string.Empty
            };

            var result = _validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.FirstName).WithErrorCode("NotEmptyValidator");
        }

        [Fact]
        public void Validate_FirstNameWithOneHundredCharacters_HasNoValidationError()
        {
            var command = CreateValidCommand() with
            {
                FirstName = new string('A', 100)
            };

            var result = _validator.TestValidate(command);

            result.ShouldNotHaveValidationErrorFor(x => x.FirstName);
        }

        [Fact]
        public void Validate_FirstNameLongerThanOneHundredCharacters_HasMaximumLengthError()
        {
            var command = CreateValidCommand() with
            {
                FirstName = new string('A', 101)
            };

            var result = _validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.FirstName).WithErrorCode("MaximumLengthValidator");
        }
        #endregion

        #region LastName
        [Fact]
        public void Validate_EmptyLastName_HasNotEmptyError()
        {
            var command = CreateValidCommand() with
            {
                LastName = string.Empty
            };

            var result = _validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.LastName).WithErrorCode("NotEmptyValidator");
        }

        [Fact]
        public void Validate_LastNameExactlyOneHundredCharacters_HasNoValidationError()
        {
            var command = CreateValidCommand() with
            {
                LastName = new string('A', 100)
            };

            var result = _validator.TestValidate(command);

            result.ShouldNotHaveValidationErrorFor(x => x.LastName);
        }

        [Fact]
        public void Validate_LastNameLongerThanOneHundredCharacters_HasMaximumLengthError()
        {
            var command = CreateValidCommand() with
            {
                LastName = new string('A', 101)
            };

            var result = _validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.LastName).WithErrorCode("MaximumLengthValidator");
        }

        #endregion

        #region Email
        [Fact]
        public void Validate_EmptyEmail_HasNotEmptyError()
        {
            var command = CreateValidCommand() with
            {
                Email = string.Empty
            };

            var result = _validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.Email).WithErrorCode("NotEmptyValidator");
        }

        [Theory]
        [InlineData("not-an-email")]
        [InlineData("missing-at-symbol.com")]
        [InlineData("@missing-local-part.com")]
        public void Validate_InvalidEmail_HasEmailAddressError(string email)
        {
            var command = CreateValidCommand() with
            {
                Email = email
            };

            var result = _validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.Email).WithErrorCode("EmailValidator");
        }

        [Theory]
        [InlineData("something@something.com")]
        [InlineData("some.thing@something.com")]
        [InlineData("some+thing@something.com.uk")]
        public void Validate_ValidEmail_HasNoValidationError(string email)
        {
            var command = CreateValidCommand() with
            {
                Email = email
            };

            var result = _validator.TestValidate(command);

            result.ShouldNotHaveValidationErrorFor(x => x.Email);
        }
        #endregion

        #region Hire Date
        [Fact]
        public void Validate_DefaultHireDate_HasNotEmptyError()
        {
            var command = CreateValidCommand() with
            {
                HireDate = default
            };

            var result = _validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.HireDate).WithErrorCode("NotEmptyValidator");
        }

        [Fact]
        public void Validate_ValidHireDate_HasNoValidationError()
        {
            var command = CreateValidCommand() with
            {
                HireDate = new DateOnly(2026, 10, 7)
            };

            var result = _validator.TestValidate(command);

            result.ShouldNotHaveValidationErrorFor(x => x.HireDate);
        }
        #endregion

        #region Department
        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-100)]
        public void Validate_NonPositiveDepartmentId_HasGreaterThanError(
            int departmentId)
        {
            var command = CreateValidCommand() with
            {
                DepartmentId = departmentId
            };

            var result = _validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.DepartmentId).WithErrorCode("GreaterThanValidator");
        }

        [Fact]
        public void Validate_DepartmentIdOfOne_HasNoValidationError()
        {
            var command = CreateValidCommand();

            var result = _validator.TestValidate(command);

            result.ShouldNotHaveValidationErrorFor(x => x.DepartmentId);
        }
        #endregion

        private static CreateEmployeeCommand CreateValidCommand()
        {
            return new CreateEmployeeCommand(
            EmployeeNumber: "EMP00001",
            FirstName: "John",
            LastName: "Smith",
            Email: "john.smith@example.com",
            HireDate: new DateOnly(2026, 10, 7),
            DepartmentId: 1);
        }
    }
}

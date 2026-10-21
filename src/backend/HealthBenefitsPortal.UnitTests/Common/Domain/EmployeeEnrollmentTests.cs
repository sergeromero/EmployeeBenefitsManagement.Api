using Benefits.Domain;
using FluentAssertions;

namespace HealthBenefitsPortal.UnitTests.Common.Domain
{
    public sealed class EmployeeEnrollmentTests
    {
        private static readonly DateOnly Today = DateOnly.FromDateTime(DateTime.UtcNow);

        [Fact]
        public void IsActiveOn_WhenEndDateIsNull_ReturnsTrue()
        {
            var enrollment = new EmployeeEnrollment
            {
                EnrollmentDate = Today.AddDays(-30)
            };

            var result = enrollment.IsActiveOn(Today);

            result.Should().BeTrue();
        }

        [Fact]
        public void IsActiveOn_WhenEndDateIsInTheFuture_ReturnsTrue()
        {
            var enrollment = CreateEnrollmentWithEndDate(Today.AddDays(5));

            var result = enrollment.IsActiveOn(Today);

            result.Should().BeTrue();
        }

        [Fact]
        public void IsActiveOn_WhenEvaluatedOnEndDate_ReturnsTrue()
        {
            var endDate = Today.AddDays(5);
            var enrollment = CreateEnrollmentWithEndDate(endDate);

            var result = enrollment.IsActiveOn(endDate);

            result.Should().BeTrue();
        }

        [Fact]
        public void IsActiveOn_WhenEndDateIsInThePast_ReturnsFalse()
        {
            var enrollment = CreateEnrollmentWithEndDate(Today.AddDays(-1));

            var result = enrollment.IsActiveOn(Today);

            result.Should().BeFalse();
        }

        [Fact]
        public void EndEnrollment_WhenGivenFutureDate_RemainsActiveUntilThatDatePasses()
        {
            var enrollment = new EmployeeEnrollment
            {
                EnrollmentDate = Today.AddDays(-30)
            };

            var endDate = Today.AddDays(5);

            enrollment.EndEnrollment(endDate);

            enrollment.EndDate.Should().Be(endDate);
            enrollment.IsActiveOn(Today).Should().BeTrue();
            enrollment.IsActiveOn(endDate).Should().BeTrue();
            enrollment.IsActiveOn(endDate.AddDays(1)).Should().BeFalse();
        }

        private static EmployeeEnrollment CreateEnrollmentWithEndDate(DateOnly endDate)
        {
            var enrollment = new EmployeeEnrollment
            {
                EnrollmentDate = Today.AddDays(-30)
            };

            enrollment.EndEnrollment(endDate);

            return enrollment;
        }
    }
}

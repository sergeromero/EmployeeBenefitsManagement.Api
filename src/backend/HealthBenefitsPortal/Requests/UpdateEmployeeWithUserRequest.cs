using Benefits.Application.Features.Employees.Common;

namespace HealthBenefitsPortal.Requests
{
    public sealed class UpdateEmployeeWithUserRequest
    {
        public EmployeeBasicInfoDto Employee { get; set; } = null!;
        public UserDto User { get; set; } = null!;
        public string Role { get; set; } = string.Empty;
    }
}

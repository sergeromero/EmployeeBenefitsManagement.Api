using Benefits.Application.Features.Employees.Common;
using Benefits.Application.Features.Employees.CreateEmployeeWithUser;

namespace HealthBenefitsPortal.Requests
{
    public sealed class CreateEmployeeWithUserRequest
    {
        public EmployeeBasicInfoDto Employee { get; set; } = null;
        public UserDto User { get; set; } = null;
        public string Role { get; set; } = string.Empty;
    }
}

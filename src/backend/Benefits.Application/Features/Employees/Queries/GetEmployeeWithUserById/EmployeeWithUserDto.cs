using Benefits.Application.Features.Employees.Common;

namespace Benefits.Application.Features.Employees.Queries.GetEmployeeWithUserById
{
    public sealed class EmployeeWithUserDto
    {
        public EmployeeBasicInfoDto Employee { get; set; } = null!;
        public UserDto? User { get; set; }
        public string? Role { get; set; }
    }
}

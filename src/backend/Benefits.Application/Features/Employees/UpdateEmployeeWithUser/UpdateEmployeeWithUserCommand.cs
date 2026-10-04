using Benefits.Application.Features.Employees.Common;
using MediatR;

namespace Benefits.Application.Features.Employees.UpdateEmployeeWithUser
{
    public sealed record UpdateEmployeeWithUserCommand(
        EmployeeBasicInfoDto Employee,
        UserDto User,
        string Role) : IRequest;
}

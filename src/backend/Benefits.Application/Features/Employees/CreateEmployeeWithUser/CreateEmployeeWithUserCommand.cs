using Benefits.Application.Features.Employees.Common;
using MediatR;

namespace Benefits.Application.Features.Employees.CreateEmployeeWithUser
{
    public sealed record CreateEmployeeWithUserCommand(
        EmployeeBasicInfoDto Employee,
        UserDto User,
        string Role) : IRequest<int>;
}

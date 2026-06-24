using MediatR;

namespace Benefits.Application.Features.Employees.CreateEmployee
{
    public sealed record CreateEmployeeCommand(
        string EmployeeNumber,
        string FirstName,
        string LastName,
        string Email,
        int DepartmentId) : IRequest<int>;
}

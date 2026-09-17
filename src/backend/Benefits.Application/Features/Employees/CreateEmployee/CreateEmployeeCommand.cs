using MediatR;

namespace Benefits.Application.Features.Employees.CreateEmployee
{
    public sealed record CreateEmployeeCommand(
        string EmployeeNumber,
        string FirstName,
        string LastName,
        string Email,
        DateOnly HireDate,
        int DepartmentId) : IRequest<int>;
}

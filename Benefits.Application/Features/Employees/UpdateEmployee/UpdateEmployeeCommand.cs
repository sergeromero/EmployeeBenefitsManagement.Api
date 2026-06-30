using MediatR;

namespace Benefits.Application.Features.Employees.UpdateEmployee
{
    public sealed record UpdateEmployeeCommand(
        int Id,
        string EmployeeNumber,
        string FirstName,
        string LastName,
        string Email,
        DateOnly HireDate,
        int DepartmentId) : IRequest
    {
    }
}

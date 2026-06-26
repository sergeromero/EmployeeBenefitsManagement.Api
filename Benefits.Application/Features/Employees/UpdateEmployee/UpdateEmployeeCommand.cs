using MediatR;

namespace Benefits.Application.Features.Employees.UpdateEmployee
{
    public sealed record UpdateEmployeeCommand(
        int Id,
        int EmployeeNumber,
        string FirstName,
        string LastName,
        string Email,
        DateTime HireDate,
        int DepartmentId) : IRequest
    {
    }
}

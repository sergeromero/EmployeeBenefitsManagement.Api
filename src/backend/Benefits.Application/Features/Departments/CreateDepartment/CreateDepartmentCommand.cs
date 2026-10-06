using MediatR;

namespace Benefits.Application.Features.Departments.CreateDepartment
{
    public record CreateDepartmentCommand(string Name) : IRequest<int>;
}

using MediatR;

namespace Benefits.Application.Features.Departments.Queries.GetDepartments
{
    public sealed record class GetDepartmentsQuery() : IRequest<List<DepartmentDto>>;
}

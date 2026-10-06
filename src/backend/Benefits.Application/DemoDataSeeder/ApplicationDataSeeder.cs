using Benefits.Application.Admin;
using Benefits.Application.Features.Administration.AssignUserToEmployee;
using Benefits.Application.Features.Departments.CreateDepartment;
using Benefits.Application.Features.Departments.Queries.GetDepartments;
using Benefits.Application.Features.Employees.CreateEmployee;
using Benefits.Application.Features.Employees.Queries.SearchEmployees;
using Benefits.Application.Infrastructure.Contracts;
using Benefits.Common;
using Benefits.Domain.Constants;
using Benefits.Domain.ReferenceData;
using MediatR;

namespace Benefits.Application.DemoDataSeeder
{
    internal sealed class ApplicationDataSeeder : IApplicationDataSeeder
    {
        private readonly IMediator _mediator;
        private readonly IIdentityService _identityService;

        public ApplicationDataSeeder(IIdentityService identityService, IMediator mediator)
        {
            _identityService = Guard.NotNull(identityService);
            _mediator = Guard.NotNull(mediator);
        }

        public async Task SeedAsync(CancellationToken cancellationToken)
        {
            var users = await _identityService.GetUsersAsync();

            var departments = await GetDepartments(cancellationToken);

            await SeedEmployees(users, departments, cancellationToken);
        }

        private async Task<List<DepartmentDto>> GetDepartments(CancellationToken cancellationToken)
        {
            List<DepartmentDto> departments = await _mediator.Send(new GetDepartmentsQuery());
           
            return departments;
        }

        private async Task SeedEmployees(List<UserDto> users, List<DepartmentDto> departments, CancellationToken cancellationToken)
        {
            var fakePeople = GetFakePeople();

            for (int i = 0; i < users.Count; i++)
            {
                var user = users[i];
                var person = fakePeople[i % fakePeople.Count];

                var employeeExists = await _mediator.Send(new SearchEmployeesQuery(string.Empty, person.EmployeeNumber, 0));

                if (employeeExists.Count > 0)
                {
                    continue;
                }

                int departmentId = await GetDepartmentId(departments, i, user);

                var employeeId = await _mediator.Send(new CreateEmployeeCommand(
                    EmployeeNumber: person.EmployeeNumber,
                    FirstName: person.FirstName,
                    LastName: person.LastName,
                    Email: user.Email,
                    HireDate: DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-30 - (i * 10))),
                    DepartmentId: departmentId),
                    cancellationToken);

                await _mediator.Send(new AssignUserToEmployeeCommand(user.Id, employeeId), cancellationToken);
            }
        }

        private async Task<int> GetDepartmentId(List<DepartmentDto> departments, int i, UserDto user)
        {
            var userRole = await _identityService.GetUserRoleAsync(user.Id);

            var nonHigherPermissionsDepartments = departments.Where(d => d.Id != DepartmentIds.HumanResources && d.Id != DepartmentIds.IT).ToList();

            if (!nonHigherPermissionsDepartments.Any())
            {
                throw new InvalidOperationException("No departments available for assignment.");
            }

            int departmentId = userRole == Roles.HR
                ? DepartmentIds.HumanResources
                : userRole == Roles.Administrator
                ? DepartmentIds.IT
                : nonHigherPermissionsDepartments[i % nonHigherPermissionsDepartments.Count].Id;

            return departmentId;
        }

        private List<(string EmployeeNumber, string FirstName, string LastName)> GetFakePeople()
        {
            return new()
            {
                ("99999001", "Lucas", "Martinez"),
                ("99999002", "Sophie", "Tremblay"),
                ("99999003", "Daniel", "Nguyen"),
                ("99999004", "Isabelle", "Gagnon"),
                ("99999005", "Carlos", "Romero"),
                ("99999006", "Emily", "Clark"),
                ("99999007", "Ahmed", "Hassan"),
                ("99999008", "Olivia", "Dubois"),
                ("99999009", "Mateo", "Lopez"),
                ("99999010", "Chloe", "Roy")
            };
        }
    }
}

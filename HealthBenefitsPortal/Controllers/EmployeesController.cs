using Benefits.Application.Features.Employees.Common;
using Benefits.Application.Features.Employees.CreateEmployee;
using Benefits.Application.Features.Employees.DeleteEmployee;
using Benefits.Application.Features.Employees.Queries.GetEmployeeById;
using Benefits.Application.Features.Employees.Queries.SearchEmployees;
using Benefits.Application.Features.Employees.Queries.ViewEmployeeBenefits;
using Benefits.Application.Features.Employees.UpdateEmployee;
using Benefits.Common;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HealthBenefitsPortal.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/employees")]
    public class EmployeesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public EmployeesController(IMediator mediator)
        {
            _mediator = Guard.NotNull(mediator);
        }

        [HttpPost]
        public async Task<ActionResult<int>> Create(CreateEmployeeCommand command)
        {
            var employeeId = await _mediator.Send(command);

            return CreatedAtAction(nameof(GetById), new { id = employeeId }, employeeId);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<EmployeeBasicInfoDto>> GetById(int id)
        {
            var employee = await _mediator.Send(new GetEmployeeByIdQuery(id));

            if(employee is null)
            {
                return NotFound();
            }

            return Ok(employee);
        }

        [HttpGet]
        public async Task<ActionResult<List<EmployeeBasicInfoDto>>> Search([FromQuery] SearchEmployeesQuery query, CancellationToken cancellationToken)
        {
            var employees = await _mediator.Send(query, cancellationToken);

            return Ok(employees);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> UpdateEmployee(int id, UpdateEmployeeDto dto)
        {
            var command = new UpdateEmployeeCommand
            (
                id,
                dto.EmployeeNumber,
                dto.FirstName,
                dto.LastName,
                dto.Email,
                dto.HireDate,
                dto.DepartmentId
            );

            await _mediator.Send(command);

            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> DeleteEmployee(int id)
        {
            var command = new DeleteEmployeeCommand(id);

            await _mediator.Send(command);
            return NoContent();
        }

        [HttpGet("{id:int}/benefits")]
        public async Task<ActionResult<EmployeeBenefitsDto>> GetEmployeeBenefits(int id, CancellationToken cancellationToken)
        {
            var employeeEnrollments = await _mediator.Send(new GetEmployeeBenefitsQuery(id), cancellationToken);

            return Ok(employeeEnrollments);
        }
    }
}

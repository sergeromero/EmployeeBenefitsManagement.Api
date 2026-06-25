using Benefits.Application.Features.Employees.Common;
using Benefits.Application.Features.Employees.CreateEmployee;
using Benefits.Application.Features.Employees.Queries.GetEmployeeById;
using Benefits.Application.Features.Employees.Queries.SearchEmployees;
using Benefits.Common;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace HealthBenefitsPortal.Controllers
{
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
    }
}

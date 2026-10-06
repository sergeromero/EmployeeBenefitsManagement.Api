using Benefits.Application.Features.Departments.Queries.GetDepartments;
using Benefits.Common;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace HealthBenefitsPortal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Departments : ControllerBase
    {
        private readonly IMediator _mediator;

        public Departments(IMediator mediator)
        {
            _mediator = Guard.NotNull(mediator);
        }

        [HttpGet]
        public async Task<ActionResult<List<DepartmentDto>>> GetDepartments()
        {
            var result = await _mediator.Send(new GetDepartmentsQuery());
            return Ok(result);
        }
    }
}

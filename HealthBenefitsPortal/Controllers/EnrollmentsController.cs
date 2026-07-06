using Benefits.Application.Features.Enrollments.CreateEnrollment;
using Benefits.Application.Features.Enrollments.TerminateEnrollment;
using Benefits.Common;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace HealthBenefitsPortal.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EnrollmentsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public EnrollmentsController(IMediator mediator)
        {
            _mediator = Guard.NotNull(mediator);
        }

        [HttpPost]
        public async Task<ActionResult> Create(CreateEnrollmentCommand command, CancellationToken cancellationToken)
        {
            await _mediator.Send(command, cancellationToken);

            return CreatedAtAction(nameof(GetEnrollment),
                new
                {
                    employeeId = command.EmployeeId,
                    benefitPlanId = command.BenefitPlanId
                },
                null);
        }

        [HttpGet("employee/{employeeId:int}/benefit-plan/{benefitPlanId:int}")]
        public Task<ActionResult> GetEnrollment(int employeeId, int benefitPlanId)
        {
            throw new NotImplementedException();
        }

        [HttpPost("terminate")]
        public async Task<ActionResult> Terminate(TerminateEnrollmentCommand command, CancellationToken cancellationToken)
        {
            //var command = new TerminateEnrollmentCommand(employeeId, benefitPlanId, endDate);
            await _mediator.Send(command, cancellationToken);

            return NoContent();
        }
    }
}

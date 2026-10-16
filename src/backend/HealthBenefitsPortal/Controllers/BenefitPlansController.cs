using Benefits.Application.Features.BenefitPlans.CreateBenefitPlans;
using Benefits.Application.Features.BenefitPlans.DeleteBenefitPlan;
using Benefits.Application.Features.BenefitPlans.Queries.GetBenefitPlans;
using Benefits.Common;
using Benefits.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HealthBenefitsPortal.Controllers
{
    [ApiController]
    [Route("api/benefitplans")]
    public class BenefitPlansController : ControllerBase
    {
        private readonly IMediator _mediator;

        public BenefitPlansController(IMediator mediator)
        {
            _mediator = Guard.NotNull(mediator);
        }

        [HttpPost]
        [Authorize(Roles = $"{Roles.Administrator},{Roles.HR}")]
        public async Task<ActionResult<int>> Create(CreateBenefitPlansCommand command)
        {
            var benefitPlanId = await _mediator.Send(command);

            return CreatedAtAction(nameof(GetById), new { id = benefitPlanId }, benefitPlanId);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult> GetById(int id)
        {
            throw new NotImplementedException();
        }

        [HttpGet]
        [Authorize(Roles = $"{Roles.Administrator},{Roles.HR},{Roles.Employee}")]
        public async Task<ActionResult<List<BenefitPlanListItemDto>>> GetBenefitsPlans(CancellationToken cancellationToken)
        {
            var benefits = await _mediator.Send(new GetBenefitPlansQuery(), cancellationToken);

            return Ok(benefits);
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = $"{Roles.Administrator},{Roles.HR}")]
        public async Task<ActionResult> Delete(int id)
        {
            var command = new DeleteBenefitPlanCommand(id);

            await _mediator.Send(command);
            return NoContent();
        }
    }
}

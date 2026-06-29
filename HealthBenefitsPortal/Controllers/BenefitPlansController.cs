using Benefits.Application.Features.BenefitPlans.CreateBenefitPlans;
using Benefits.Application.Features.BenefitPlans.Queries.GetBenefitPlans;
using Benefits.Common;
using MediatR;
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
        public async Task<ActionResult<List<BenefitPlanListItemDto>>> GetBenefitsPlans(CancellationToken cancellationToken)
        {
            var benefits = await _mediator.Send(new GetBenefitPlansQuery(), cancellationToken);

            return Ok(benefits);
        }
    }
}

using Benefits.Application.Features.BenefitTypes.CreateBenefitTypes;
using Benefits.Application.Features.BenefitTypes.Queries.GetBenefitPlansByType;
using Benefits.Common;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace HealthBenefitsPortal.Controllers
{
    [ApiController]
    [Route("api/benefittypes")]
    public class BenefitTypesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public BenefitTypesController(IMediator mediator)
        {
            _mediator = Guard.NotNull(mediator);
        }

        [HttpPost]
        public async Task<ActionResult<int>> Create(CreateBenefitTypesCommand command)
        {
            var benefitTypeId = await _mediator.Send(command);

            return CreatedAtAction(nameof(GetById), new { id = benefitTypeId }, benefitTypeId);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult> GetById(int id)
        {
            throw new NotImplementedException();
        }

        [HttpGet("{id:int}/plans")]
        public async Task<ActionResult<IReadOnlyList<BenefitPlanListItemDto>>> GetPlans([FromRoute] GetBenefitPlansByTypeQuery query)
        {
            var plans = await _mediator.Send(query);

            return Ok(plans);
        }
    }
}

using Benefits.Application.Features.BenefitTypes.CreateBenefitTypes;
using Benefits.Common;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace HealthBenefitsPortal.Controllers
{
    [ApiController]
    [Route("api/benefittypes")]
    public class BenefitTypes : ControllerBase
    {
        private readonly IMediator _mediator;

        public BenefitTypes(IMediator mediator)
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
    }
}

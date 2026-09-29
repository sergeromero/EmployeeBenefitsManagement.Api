using Benefits.Application.Features.BenefitPlans.Queries.GetUserRoles;
using Benefits.Common;
using Benefits.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HealthBenefitsPortal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public RolesController(IMediator mediator)
        {
            _mediator = Guard.NotNull(mediator);
        }

        [HttpGet]
        [Authorize(Roles = Roles.Administrator)]
        public async Task<IActionResult> GetRoles(CancellationToken cancellationToken)
        {
            var roles = await _mediator.Send(new GetUserRolesQuery(), cancellationToken);

            return Ok(roles);
        }
    }
}

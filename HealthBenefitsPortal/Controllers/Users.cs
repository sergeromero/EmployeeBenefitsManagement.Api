using Benefits.Application.Features.Administration.AssignRoleToUser;
using Benefits.Application.Infrastructure.Contracts;
using Benefits.Common;
using Benefits.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HealthBenefitsPortal.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class Users : ControllerBase
    {
        private readonly IMediator _mediator;

        public Users(IMediator mediator)
        {
            _mediator = Guard.NotNull(mediator);
        }

        [Authorize(Roles = Roles.Administrator)]
        [HttpGet]
        public async Task<IActionResult> GetUsers([FromServices] IIdentityService identityService)
        {
            var users = await identityService.GetUsersAsync();
            return Ok(users);
        }

        [Authorize(Roles = Roles.Administrator)]
        [HttpPost("assign-role")]
        public async Task<IActionResult> AssignRole(AssignRoleToUserCommand command, CancellationToken cancellationToken)
        {
            await _mediator.Send(command);
            return NoContent();
        }
    }
}

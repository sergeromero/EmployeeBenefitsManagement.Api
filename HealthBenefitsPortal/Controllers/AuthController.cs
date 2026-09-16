using Benefits.Application.Authentication.DTOs;
using Benefits.Application.Authentication.Interfaces;
using Benefits.Common;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace HealthBenefitsPortal.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public sealed class AuthController : ControllerBase
    {
        private readonly IAuthenticationService _authenticationService;

        public AuthController(IAuthenticationService authenticationService, IMediator mediator)
        {
            _authenticationService = Guard.NotNull(authenticationService);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
        {
            var result = await _authenticationService.AuthenticateAsync(request, cancellationToken);

            if(result is null)
            {
                return Unauthorized(new
                {
                    message = "Invalid email or password."
                });
            }

            return Ok(result);
        }
    }
}

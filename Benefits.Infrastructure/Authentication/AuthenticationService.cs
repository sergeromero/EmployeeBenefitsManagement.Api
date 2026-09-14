using Benefits.Application.Authentication.DTOs;
using Benefits.Application.Authentication.Interfaces;
using Benefits.Common;
using Benefits.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;

namespace Benefits.Infrastructure.Authentication
{
    public sealed class AuthenticationService : IAuthenticationService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;

        public AuthenticationService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IJwtTokenGenerator jwtTokenGenerator)
        {
            _userManager = Guard.NotNull(userManager);
            _signInManager = Guard.NotNull(signInManager);
            _jwtTokenGenerator = Guard.NotNull(jwtTokenGenerator);
        }

        public async Task<LoginResponse?> AuthenticateAsync(LoginRequest request, CancellationToken cancellationToken = default)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if(user is null || string.IsNullOrWhiteSpace(user.Email))
            {
                return null;
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: false);
            if (!result.Succeeded)
            {
                return null;
            }

            var roles = await _userManager.GetRolesAsync(user);

            var token = _jwtTokenGenerator.GenerateToken(user.Id, user.Email!, roles);
            var expiresAt = _jwtTokenGenerator.GetExpiration();

            return new LoginResponse
            {
                AccessToken = token,
                ExpiresAt = expiresAt,
                User = new AuthenticatedUser
                {
                    Id = user.Id,
                    Email = user.Email,
                    Roles = roles.ToList()
                }
            };
        }
    }
}

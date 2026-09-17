using Benefits.Application.Authentication.Interfaces;
using Benefits.Common;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Benefits.Infrastructure.Authentication
{
    public sealed class JwtTokenGenerator : IJwtTokenGenerator
    {
        private readonly JwtOptions _jwtOptions;
        private readonly TimeProvider _timeProvider;

        public JwtTokenGenerator(IOptions<JwtOptions> options, TimeProvider timeProvider)
        {
            var received = Guard.NotNull(options);
            _jwtOptions = received.Value;

            _timeProvider = Guard.NotNull(timeProvider);
        }

        public string GenerateToken(string userId, string email, IEnumerable<string> roles)
        {
            var claims = new List<Claim>
            {
                new (JwtRegisteredClaimNames.Sub, userId),
                new (JwtRegisteredClaimNames.Email, email),
                new (ClaimTypes.NameIdentifier, userId),
                new (ClaimTypes.Email, email),
                new (JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Key));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = GetExpiration();

            var token = new JwtSecurityToken(
                issuer: _jwtOptions.Issuer,
                audience: _jwtOptions.Audience,
                claims: claims,
                expires: expires,
                signingCredentials: credentials);

            var tokenHandler = new JwtSecurityTokenHandler();

            return tokenHandler.WriteToken(token);
        }

        public DateTime GetExpiration()
        {
            return _timeProvider.GetUtcNow().AddMinutes(_jwtOptions.ExpirationMinutes).UtcDateTime;
        }
    }
}

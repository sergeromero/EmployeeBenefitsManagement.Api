using Benefits.Application.Infrastructure.Contracts;
using Benefits.Common;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Benefits.Infrastructure.Services
{
    public sealed class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _contextAccessor;

        public CurrentUserService(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = Guard.NotNull(contextAccessor);
        }

        public string? UserId => _contextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);

        public bool IsInRole(string role)
        {
            return _contextAccessor.HttpContext?.User?.IsInRole(role) ?? false;
        }
    }
}

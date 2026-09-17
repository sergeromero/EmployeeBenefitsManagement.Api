using Benefits.Application.Authentication.DTOs;

namespace Benefits.Application.Authentication.Interfaces
{
    public interface IAuthenticationService
    {
        Task<LoginResponse?> AuthenticateAsync(LoginRequest request, CancellationToken cancellationToken = default);
    }
}

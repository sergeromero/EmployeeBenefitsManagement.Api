namespace Benefits.Application.Authentication.DTOs
{
    public sealed class LoginResponse
    {
        public string AccessToken { get; init; } = string.Empty;
        public DateTime ExpiresAt { get; init; }
        public AuthenticatedUser User { get; init; } = new();
    }
}

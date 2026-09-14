namespace Benefits.Application.Authentication.Interfaces
{
    public interface IJwtTokenGenerator
    {
        string GenerateToken(string userId, string email, IEnumerable<string> roles);

        DateTime GetExpiration();
    }
}

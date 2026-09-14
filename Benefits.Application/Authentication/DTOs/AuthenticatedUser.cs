namespace Benefits.Application.Authentication.DTOs
{
    public sealed class AuthenticatedUser
    {
        public string Id { get; init; } = string.Empty;
        public string Email {  get; init; } = string.Empty;
        public IReadOnlyList<string> Roles { get; init; } = new List<string>();
    }
}
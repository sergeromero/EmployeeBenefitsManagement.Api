namespace Benefits.Application.Infrastructure.Contracts
{
    public interface ICurrentUserService
    {
        string? UserId { get; }
        bool IsInRole(string role);
    }
}

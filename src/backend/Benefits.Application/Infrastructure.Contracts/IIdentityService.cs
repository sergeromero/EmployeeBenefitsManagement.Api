using Benefits.Application.Admin;

namespace Benefits.Application.Infrastructure.Contracts
{
    public interface IIdentityService
    {
        Task AssignRoleToUserAsync(string userId, string role);
        Task<bool> RoleExistsAsync(string role);

        Task<List<UserDto>> GetUsersAsync();
    }
}

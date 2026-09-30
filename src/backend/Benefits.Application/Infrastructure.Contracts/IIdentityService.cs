using Benefits.Application.Admin;

namespace Benefits.Application.Infrastructure.Contracts
{
    public interface IIdentityService
    {
        Task<string> CreateUserAsync(string userName, string email, string password);
        Task AssignRoleToUserAsync(string userId, string role);
        Task<bool> RoleExistsAsync(string role);
        Task<bool> UserExistsByEmaiAsync(string email);
        Task<List<UserDto>> GetUsersAsync();
        Task<List<RoleDto>> GetRolesAsync();
        Task DeleteUserAsync(string userId);
    }
}

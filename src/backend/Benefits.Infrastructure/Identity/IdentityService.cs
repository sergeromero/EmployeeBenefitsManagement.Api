using System.Linq;
using Benefits.Application.Admin;
using Benefits.Application.Exceptions;
using Benefits.Application.Exceptions.BusinessRuleViolationException;
using Benefits.Application.Infrastructure.Contracts;
using Benefits.Common;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Benefits.Infrastructure.Identity
{
    internal class IdentityService : IIdentityService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public IdentityService(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = Guard.NotNull(userManager);
            _roleManager = Guard.NotNull(roleManager);
        }

        public async Task AssignRoleToUserAsync(string userId, string role)
        {
            var user = await FindUserByIdAsync(userId);

            if (!await _roleManager.RoleExistsAsync(role))
            {
                throw new NotFoundException($"Role '{role}' was not found.");
            }

            var currentRoles = await _userManager.GetRolesAsync(user);
            var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
            VerifyIdentityResult(removeResult);

            var result = await _userManager.AddToRoleAsync(user, role);
            VerifyIdentityResult(result);
        }

        public async Task<string> CreateUserAsync(string userName, string email, string password)
        {
            var user = new ApplicationUser
            {
                UserName = userName,
                Email = email,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, password);
            VerifyIdentityResult(result);

            return user.Id;
        }

        public async Task<List<RoleDto>> GetRolesAsync()
        {
            var roles = _roleManager.Roles.Select(r => new RoleDto
            {
                Id = r.Id,
                Name = r.Name
            }).OrderBy(r => r.Name).ToList();

            return roles;
        }

        public async Task<List<UserDto>> GetUsersAsync()
        {
            var users = _userManager.Users.ToList();

            return users.Select(u => new UserDto
            {
                Id = u.Id,
                Email = u.Email
            }).ToList();
        }

        public async Task<bool> RoleExistsAsync(string role)
        {
            return await _roleManager.RoleExistsAsync(role);
        }

        public async Task<bool> UserExistsByEmaiAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email) != null;
        }

        public async Task DeleteUserAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null) return;

            var result = await _userManager.DeleteAsync(user);
            VerifyIdentityResult(result);
        }

        public async Task<Dictionary<string, string>> GetUserNamesByIdsAsync(IEnumerable<string?> userIds, CancellationToken cancellationToken)
        {
            var ids = userIds.Where(id => id != null).Distinct().ToList();

            var users = await _userManager.Users
                .Where(u => ids.Contains(u.Id))
                .Select(u => new { u.Id, u.UserName })
                .ToListAsync(cancellationToken);

            return users.ToDictionary(u => u.Id, u => u.UserName!);
        }

        public async Task UpdateUserAsync(string userId, string userName, string email)
        {
            var user = await FindUserByIdAsync(userId);

            user.UserName = userName;
            user.Email = email;
            user.EmailConfirmed = true;
            user.NormalizedUserName = _userManager.NormalizeName(userName);
            user.NormalizedEmail = _userManager.NormalizeEmail(email);

            var securityStampResult = await _userManager.UpdateSecurityStampAsync(user);
            VerifyIdentityResult(securityStampResult);

            var updateResult = await _userManager.UpdateAsync(user);
            VerifyIdentityResult(updateResult);
        }

        public async Task UpdatePasswordAsync(string userId, string password)
        {
            var user = await FindUserByIdAsync(userId);

            var removeResult = await _userManager.RemovePasswordAsync(user);
            VerifyIdentityResult(removeResult);

            var addResult = await _userManager.AddPasswordAsync(user, password);
            VerifyIdentityResult(addResult);

            await _userManager.UpdateSecurityStampAsync(user);
        }

        public async Task<Application.Features.Employees.Common.UserDto> GetUserByIdAsync(string userId)
        {
            var user = await FindUserByIdAsync(userId);

            return new Application.Features.Employees.Common.UserDto
            {
                Email = user.Email,
                UserName = user.UserName
            };
        }
        public async Task<string> GetUserRoleAsync(string userId)
        {
            var user = await FindUserByIdAsync(userId);
            var result = await _userManager.GetRolesAsync(user);

            return result.FirstOrDefault();
        }

        private static void VerifyIdentityResult(IdentityResult result)
        {
            if (!result.Succeeded)
            {
                throw new IdentityOperationException(string.Join(", ", result.Errors.Select(e => e.Description)));
            }
        }

        private async Task<ApplicationUser> FindUserByIdAsync(string userId)
        {
            return await _userManager.FindByIdAsync(userId) ?? throw new NotFoundException("User not found");
        }

    }
}

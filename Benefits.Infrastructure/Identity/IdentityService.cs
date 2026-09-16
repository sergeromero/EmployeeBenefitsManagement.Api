using Benefits.Application.Admin;
using Benefits.Application.Exceptions;
using Benefits.Application.Exceptions.BusinessRuleViolationException;
using Benefits.Application.Infrastructure.Contracts;
using Benefits.Common;
using Microsoft.AspNetCore.Identity;

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
            var user = await _userManager.FindByIdAsync(userId) ?? throw new NotFoundException("User not found");

            if(!await _roleManager.RoleExistsAsync(role))
            {
                throw new NotFoundException($"Role '{role}' was not found.");
            }

            if(!await _userManager.IsInRoleAsync(user, role))
            {
                var result = await _userManager.AddToRoleAsync(user, role);

                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    throw new IdentityOperationException(errors);
                }
            }
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

        public Task<bool> RoleExistsAsync(string role)
        {
            return _roleManager.RoleExistsAsync(role);
        }
    }
}

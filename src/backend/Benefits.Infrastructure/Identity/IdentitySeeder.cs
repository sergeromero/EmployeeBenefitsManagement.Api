using Benefits.Application.Exceptions;
using Benefits.Common;
using Benefits.Domain.Constants;
using Benefits.Infrastructure.Configuration;
using Benefits.Infrastructure.Configuration.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Benefits.Infrastructure.Identity
{
    internal sealed class IdentitySeeder
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IOptions<DefaultAdministratorOptions> _defaultAdministrator;
        private readonly IOptions<SeedOptions> _seedOptions;
        private readonly ILogger<IdentitySeeder> _logger;

        public IdentitySeeder(UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IOptions<DefaultAdministratorOptions> administratorOptions,
            IOptions<SeedOptions> seedOptions,
            ILogger<IdentitySeeder> logger)
        {
            _userManager = Guard.NotNull(userManager);
            _roleManager = Guard.NotNull(roleManager);
            _defaultAdministrator = Guard.NotNull(administratorOptions);
            _seedOptions = Guard.NotNull(seedOptions);
            _logger = Guard.NotNull(logger);
        }

        public async Task SeedAsync()
        {
            _logger.LogInformation("Initializing ASP.NET Identity.");

            await InitializeRolesAsync();
            await InitializeAdministratorAsync();
            await InitializeTestUsers();

            _logger.LogInformation("ASP.NET Identity initialization completed.");
        }

        private async Task InitializeRolesAsync()
        {
            foreach(var roleName in Roles.All)
            {
                if(await _roleManager.RoleExistsAsync(roleName))
                {
                    continue;
                }

                _logger.LogInformation("Creating role {roleName}", roleName);

                var result = await _roleManager.CreateAsync(new IdentityRole(roleName));

                ThrowIfFailed(result);
            }
        }

        private async Task InitializeAdministratorAsync()
        {
            var administrator = await _userManager.FindByEmailAsync(_defaultAdministrator.Value.Email);

            if(administrator is null)
            {
                _logger.LogInformation("Creating default administrator account");

                administrator = new ApplicationUser
                {
                    UserName = _defaultAdministrator.Value.UserName,
                    Email = _defaultAdministrator.Value.Email,
                    EmailConfirmed = true
                };

                var result = await _userManager.CreateAsync(administrator, _defaultAdministrator.Value.Password);
                ThrowIfFailed(result);
            }

            if(!await _userManager.IsInRoleAsync(administrator, Roles.Administrator))
            {
                _logger.LogInformation("Assigning role '{RoleName}' to the default administrator.", Roles.Administrator);

                var addToRoleResult = await _userManager.AddToRoleAsync(administrator, Roles.Administrator);

                ThrowIfFailed(addToRoleResult);
            }
        }

        private async Task InitializeTestUsers()
        {
            if (_seedOptions.Value.IncludeTestUsers)
            {
                var user = await EnsureUserAsync("hr1@test.com", "Password123!");
                await EnsureUserInRole(user, Roles.HR);

                user = await EnsureUserAsync("hr2@test.com", "Password123!");
                await EnsureUserInRole(user, Roles.HR);

                user = await EnsureUserAsync("employee1@test.com", "Password123!");
                await EnsureUserInRole(user, Roles.Employee);

                user = await EnsureUserAsync("employee2@test.com", "Password123!");
                await EnsureUserInRole(user, Roles.Employee);

                user = await EnsureUserAsync("hr3@test.com", "Password123!");
                await EnsureUserInRole(user, Roles.HR);

                user = await EnsureUserAsync("employee3@test.com", "Password123!");
                await EnsureUserInRole(user, Roles.Employee);

                user = await EnsureUserAsync("employee4@test.com", "Password123!");
                await EnsureUserInRole(user, Roles.Employee);

                user = await EnsureUserAsync("employee5@test.com", "Password123!");
                await EnsureUserInRole(user, Roles.Employee);
            }
        }

        private async Task<ApplicationUser> EnsureUserAsync(string email, string password)
        {
            var existingUser = await _userManager.FindByEmailAsync(email);

            if(existingUser != null)
            {
                return existingUser;
            }

            var user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, password);
            ThrowIfFailed(result);

            return user;
        }

        private async Task EnsureUserInRole(ApplicationUser user, string role)
        {
            if (!await _roleManager.RoleExistsAsync(role))
            {
                throw new NotFoundException($"Role '{role}' was not found.");
            }

            if (await _userManager.IsInRoleAsync(user, role))
            {
                return;
            }

            var result = await _userManager.AddToRoleAsync(user, role);
            ThrowIfFailed(result);
        }

        private void ThrowIfFailed(IdentityResult result)
        {
            if (!result.Succeeded)
            {
                var errors = string.Join(Environment.NewLine, result.Errors.Select(error => error.Description));

                throw new IdentitySeedingException($"Failed to initialize ASP.NET Identity.{Environment.NewLine}{errors}");
            }
        }
    }
}

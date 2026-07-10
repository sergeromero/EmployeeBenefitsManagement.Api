using Benefits.Common;
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
        private readonly ILogger<IdentitySeeder> _logger;

        public IdentitySeeder(UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IOptions<DefaultAdministratorOptions> administratorOptions,
            ILogger<IdentitySeeder> logger)
        {
            _userManager = Guard.NotNull(userManager);
            _roleManager = Guard.NotNull(roleManager);
            _defaultAdministrator = Guard.NotNull(administratorOptions);
            _logger = Guard.NotNull(logger);
        }

        public async Task SeedAsync()
        {
            _logger.LogInformation("Initializing ASP.NET Identity.");

            await InitializeRolesAsync();
            await InitializeAdministratorAsync();

            _logger.LogInformation("ASP.NET Identity initialization completed.");
        }

        private async Task InitializeRolesAsync()
        {
            foreach(var roleName in IdentityRoles.All)
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

            if(!await _userManager.IsInRoleAsync(administrator, IdentityRoles.Administrator))
            {
                _logger.LogInformation("Assigning role '{RoleName}' to the default administrator.", IdentityRoles.Administrator);

                var addToRoleResult = await _userManager.AddToRoleAsync(administrator, IdentityRoles.Administrator);

                ThrowIfFailed(addToRoleResult);
            }
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

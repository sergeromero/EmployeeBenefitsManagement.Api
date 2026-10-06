using Benefits.Application.DemoDataSeeder;
using Benefits.Infrastructure.Identity;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Benefits.Infrastructure.Configuration
{
    public static class WebApplicationExtensions
    {
        public static async Task InitializeInfrastructureAsync(this WebApplication app)
        {
            ArgumentNullException.ThrowIfNull(app);

            await using var scope = app.Services.CreateAsyncScope();

            var serviceProvider = scope.ServiceProvider;

            var identitySeeder = serviceProvider.GetRequiredService<IdentitySeeder>();

            await identitySeeder.SeedAsync();
        }

        public static async Task InitializeApplicationAsync(this WebApplication app)
        {
            ArgumentNullException.ThrowIfNull(app);

            await using var scope = app.Services.CreateAsyncScope();
            var serviceProvider = scope.ServiceProvider;

            var options = serviceProvider.GetRequiredService<IOptions<SeedOptions>>();

            if (!options.Value.IncludeDemoData)
            {
                return;
            }

            var appSeeder = serviceProvider.GetRequiredService<IApplicationDataSeeder>();

            await appSeeder.SeedAsync(CancellationToken.None);
        }
    }
}

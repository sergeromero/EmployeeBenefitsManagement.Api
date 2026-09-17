using Benefits.Infrastructure.Identity;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

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
    }
}

using Benefits.Application.Infrastructure.Contracts;
using Benefits.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Scrutor;

namespace Benefits.Infrastructure.DI
{
    public static class DependencyInjection
    {
        public static IServiceCollection RegisterInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<BenefitsDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            services.Scan(scan => scan
                .FromAssemblyOf<InfrastructureAssemblyMarker>()
                .AddClasses(classes => classes.Where(type => type.Name.EndsWith("Repository")))
                .UsingRegistrationStrategy(RegistrationStrategy.Skip)
                .AsImplementedInterfaces()
                .WithScopedLifetime());

            services.AddScoped<IBenefitsDbContext, BenefitsDbContext>();

            return services;
        }
    }
}

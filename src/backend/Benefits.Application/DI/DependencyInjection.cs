using Benefits.Application.Application.Contracts;
using Benefits.Application.Behaviors;
using Benefits.Application.Services;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Scrutor;

namespace Benefits.Application.DI
{
    public static class DependencyInjection
    {
        public static IServiceCollection RegisterApplication(this IServiceCollection services)
        {
            services.Scan(scan => scan
            .FromAssemblyOf<ApplicationAssemblyMarker>()
            .AddClasses(classes => classes.Where(type => type.Name.EndsWith("Service")))
            .UsingRegistrationStrategy(RegistrationStrategy.Skip)
            .AsImplementedInterfaces()
            .WithScopedLifetime());

            services.AddMediatR(configuration =>
            {
                configuration.RegisterServicesFromAssembly(typeof(ApplicationAssemblyMarker).Assembly);
                configuration.AddOpenBehavior(typeof(ValidationBehavior<,>));
            });

            services.AddValidatorsFromAssembly(typeof(ApplicationAssemblyMarker).Assembly);

            services.AddScoped<IProvisioningService, ProvisioningService>();

            return services;
        }
    }
}

using Microsoft.Extensions.DependencyInjection;
using Scrutor;
using FluentValidation;
using Benefits.Application.Behaviors;

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

            return services;
        }
    }
}

using Benefits.Application.Infrastructure.Contracts;
using Benefits.Infrastructure.Configuration.Identity;
using Benefits.Infrastructure.Identity;
using Benefits.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
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

            services.AddOptions<DefaultAdministratorOptions>()
                .Bind(configuration.GetSection(DefaultAdministratorOptions.SectionName))
                .ValidateDataAnnotations()
                .ValidateOnStart();

            services.AddDataProtection();

            services.AddIdentityCore<ApplicationUser>(ConfigureIdentityOptions)
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<BenefitsDbContext>()
                .AddDefaultTokenProviders();

            services.AddScoped<IdentitySeeder>();

            services.Scan(scan => scan
                .FromAssemblyOf<InfrastructureAssemblyMarker>()
                .AddClasses(classes => classes.Where(type => type.Name.EndsWith("Repository")))
                .UsingRegistrationStrategy(RegistrationStrategy.Skip)
                .AsImplementedInterfaces()
                .WithScopedLifetime());

            services.AddScoped<IBenefitsDbContext, BenefitsDbContext>();
            services.AddSingleton(TimeProvider.System);

            return services;
        }

        private static void ConfigureIdentityOptions(IdentityOptions options)
        {
            // Password
            options.Password.RequiredLength = 8;
            options.Password.RequireUppercase = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireDigit = true;
            options.Password.RequireNonAlphanumeric = false;

            // Lockout
            options.Lockout.AllowedForNewUsers = true;
            options.Lockout.MaxFailedAccessAttempts = 5;
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);

            // User
            options.User.RequireUniqueEmail = true;
        }
    }
}

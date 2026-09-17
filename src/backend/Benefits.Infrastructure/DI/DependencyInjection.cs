using Benefits.Application.Authentication.Interfaces;
using Benefits.Application.Infrastructure.Contracts;
using Benefits.Infrastructure.Authentication;
using Benefits.Infrastructure.Configuration;
using Benefits.Infrastructure.Configuration.Identity;
using Benefits.Infrastructure.Identity;
using Benefits.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Scrutor;
using System.Text;

namespace Benefits.Infrastructure.DI
{
    public static class DependencyInjection
    {
        public static IServiceCollection RegisterInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<SeedOptions>(configuration.GetSection("SeedOptions"));

            services.AddDbContext<BenefitsDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            services.AddOptions<DefaultAdministratorOptions>()
                .Bind(configuration.GetSection(DefaultAdministratorOptions.SectionName))
                .ValidateDataAnnotations()
                .ValidateOnStart();

            services.AddDataProtection();

            services.AddIdentityCore<ApplicationUser>(ConfigureIdentityOptions)
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<BenefitsDbContext>()
                .AddSignInManager()
                .AddDefaultTokenProviders();

            services.AddScoped<IdentitySeeder>();

            services.Scan(scan => scan
                .FromAssemblyOf<InfrastructureAssemblyMarker>()
                .AddClasses(classes => classes.Where(type => type.Name.EndsWith("Repository")))
                .UsingRegistrationStrategy(RegistrationStrategy.Skip)
                .AsImplementedInterfaces()
                .WithScopedLifetime());

            services.AddScoped<IIdentityService, IdentityService>();

            services.AddScoped<IBenefitsDbContext, BenefitsDbContext>();
            services.AddSingleton(TimeProvider.System);

            services.AddOptions<JwtOptions>()
                .Bind(configuration.GetSection(JwtOptions.SectionName))
                .Validate(options =>
                    !string.IsNullOrWhiteSpace(options.Key) && options.Key.Length >= 32,
                    "Jwt Key must be at least 32 characters long")
                .ValidateOnStart();

            services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
            services.AddScoped<IAuthenticationService, AuthenticationService>();

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            });

            services.AddAuthentication().AddJwtBearer(options =>
            {
                var jwtOptions = configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>();

                if (jwtOptions is null || string.IsNullOrWhiteSpace(jwtOptions.Key))
                {
                    throw new InvalidOperationException("JWT configuration is missing or invalid. Verify that the 'Jwt' section exists and that 'Jwt:Key' is configured (e.g., via User Secrets or environment variables).");
                }

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwtOptions.Issuer,

                    ValidateAudience = true,
                    ValidAudience = jwtOptions.Audience,

                    ValidateLifetime = true,

                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Key)),

                    ClockSkew = TimeSpan.Zero
                };
            });

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

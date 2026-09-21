using Benefits.Application.DI;
using Benefits.Infrastructure.Configuration;
using Benefits.Infrastructure.DI;
using HealthBenefitsPortal.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.RegisterApplication();
builder.Services.RegisterInfrastructure(builder.Configuration);

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddControllers();

var allowedOrigins = builder.Configuration
    .GetSection("Cors:AllowedOrigins")
    .Get<string[]>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("DefaultCorsPolicy",
        policy =>
        {
            policy
                .WithOrigins(allowedOrigins!)
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseExceptionHandler();

app.UseHttpsRedirection();

app.UseCors("DefaultCorsPolicy");
app.UseAuthentication();
app.UseAuthorization();

await app.InitializeInfrastructureAsync();

app.MapControllers();

app.Run();

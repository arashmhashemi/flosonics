using Api.Mappers;
using Api.Repository.Session;
using Api.Services;
using FluentValidation;

namespace Api.Startup;
public static class Services
{
    public static void RegisterServices(this WebApplicationBuilder builder)
    {
        // Register the swagger generator
        builder.Services
            .AddEndpointsApiExplorer()
            .AddSwaggerGen();

        // Register the validators
        builder.Services.AddValidatorsFromAssemblyContaining<Program>();

        // Register the session endpoint and its dependencies
        builder.Services
            .AddScoped<ISessionService, SessionService>()

            // The repository is a singleton to make sure that the data is not lost when the endpoint is disposed
            .AddSingleton<ISessionRepository, InMemorySessionRepository>()

            // The mapper is a singleton for performance reasons and it is stateless
            .AddSingleton<ISessionMapper, SessionMapper>();
    }
}
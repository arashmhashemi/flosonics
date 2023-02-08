namespace Api.Startup;

public static class Routes
{
    public static void RegisterRoutes(this WebApplication app)
    {
        app.RegisterSessionRoutes();
    }
}
using Api.Filters;
using Api.Model;
using Api.Model.Session;
using Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Api.Startup;

public static class SessionRoutes
{
    public static void RegisterSessionRoutes(this WebApplication app)
    {
        app.MapGet("/session/{id}", GetSessionById)
            .Produces<SessionResponse>()
            .Produces(StatusCodes.Status404NotFound);

        app.MapGet("/session", GetAllSessions)
            .Produces<PagedList<SessionResponse>>();

        app.MapPost("/session", CreateSession)
            .AddEndpointFilter<ValidatorFilter<SessionRequest>>()
            .Produces<SessionResponse>()
            .Produces(StatusCodes.Status201Created)
            .ProducesValidationProblem();

        app.MapPut("/session/{id}", UpdateSession)
            .AddEndpointFilter<ValidatorFilter<SessionRequest>>()
            .Produces<SessionResponse>()
            .ProducesValidationProblem()
            .Produces(StatusCodes.Status412PreconditionFailed)
            .Produces(StatusCodes.Status404NotFound);

        app.MapDelete("/session/{id}", DeleteSession)
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status412PreconditionFailed);

        app.MapGet("/session/average", GetAverageDuration)
            .Produces<SessionAverageDurationResponse>();
    }

    private static Task<IResult> GetAverageDuration(
        [FromServices] ISessionService service,
        [FromQuery] DateTimeOffset from,
        [FromQuery] DateTimeOffset to)
    {
        return service.GetAverageDuration(from, to);
    }

    private static Task<IResult> DeleteSession(
        [FromServices] ISessionService service,
        [FromRoute] string id,
        [FromHeader(Name = "If-Match")] string etag)
    {
        return service.Delete(id, etag);
    }

    private static Task<IResult> UpdateSession(
        HttpContext httpContext,
        [FromServices] ISessionService service,
        [FromRoute] string id,
        [FromBody] SessionRequest request,
        [FromHeader(Name = "If-Match")] string etag)
    {
        return service.Put(httpContext.Response, request, id, etag);
    }

    private static Task<IResult> CreateSession(
        HttpContext httpContext,
        [FromServices] ISessionService service,
        SessionRequest request)
    {
        return service.Post(httpContext.Response, request);
    }

    private static Task<IResult> GetAllSessions(
        [FromServices] ISessionService service,
        [FromQuery] string? name,
        [FromQuery] string? tag,
        [FromQuery] int? page,
        [FromQuery] int? pageSize
        )
    {
        return service.Get(new SessionSearch
        {
            Name = name,
            Tag = tag
        }, new Paging
        {
            Page = page ?? 0,
            PageSize = pageSize ?? 10
        });
    }

    private static Task<IResult> GetSessionById(
        HttpContext httpContext,
        [FromServices] ISessionService service,
        [FromRoute] string id)
    {
        return service.Get(httpContext.Response, id);
    }
}

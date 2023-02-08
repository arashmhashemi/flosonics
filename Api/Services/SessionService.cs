using Api.Exceptions;
using Api.Mappers;
using Api.Model;
using Api.Model.Session;
using Api.Repository.Session;

namespace Api.Services;

public interface ISessionService
{
    Task<IResult> Get(HttpResponse httpResponse, string id);
    Task<IResult> Get(SessionSearch search, Paging paging);
    Task<IResult> Post(HttpResponse httpResponse, SessionRequest request);
    Task<IResult> Put(HttpResponse httpResponse, SessionRequest request, string id, string etag);
    Task<IResult> Delete(string id, string etag);
    Task<IResult> GetAverageDuration(DateTimeOffset fromDateAdded, DateTimeOffset toDateAdded);
}

public class SessionService : ISessionService
{
    private readonly ISessionRepository _repository;
    private readonly ISessionMapper _mapper;

    public SessionService(ISessionRepository repository, ISessionMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<IResult> Get(HttpResponse httpResponse, string id)
    {
        var entity = await _repository.GetAsync(id);
        if (entity is null)
        {
            return Results.NotFound();
        }

        httpResponse.Headers.Add("ETag", entity.ETag);
        return TypedResults.Ok(_mapper.Map(entity));
    }

    public async Task<IResult> Get(SessionSearch search, Paging paging)
    {
        var result = await _repository.GetAllAsync(paging, search.Name, search.Tag);

        if (result.Total > (result.Page + 1) * result.PageSize)
        {
            result.Next = $"/session?page={result.Page + 1}&pageSize={result.PageSize}&name={search.Name}&tag={search.Tag}";
        }

        return TypedResults.Ok(_mapper.Map(result));
    }

    public async Task<IResult> Post(HttpResponse httpResponse, SessionRequest request)
    {
        var entity = _mapper.Map(request);

        await _repository.AddAsync(entity);
        httpResponse.Headers.Add("ETag", entity.ETag);

        return TypedResults.Created($"/session/{entity.Id}", _mapper.Map(entity));
    }

    public async Task<IResult> Put(HttpResponse httpResponse, SessionRequest request, string id, string etag)
    {
        var entity = _mapper.Map(request);
        entity.Id = id;
        try
        {
            entity = await _repository.UpdateAsync(entity, etag);
            httpResponse.Headers.Add("ETag", entity.ETag);
        }
        catch (EntityModifiedException)
        {
            return Results.StatusCode(StatusCodes.Status412PreconditionFailed);
        }
        catch (EntityNotFoundException)
        {
            return Results.NotFound();
        }

        return TypedResults.Ok(_mapper.Map(entity));
    }

    public async Task<IResult> Delete(string id, string etag)
    {
        try
        {
            await _repository.DeleteAsync(id, etag);
        }
        catch (EntityModifiedException)
        {
            return Results.StatusCode(StatusCodes.Status412PreconditionFailed);
        }

        return Results.NoContent();
    }

    public async Task<IResult> GetAverageDuration(DateTimeOffset fromDateAdded, DateTimeOffset toDateAdded)
    {
        var result = await _repository.GetAverageDurationAsync(fromDateAdded, toDateAdded);

        return TypedResults.Ok(new SessionAverageDurationResponse { AverageDurationSeconds = result });
    }
}

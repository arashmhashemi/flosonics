using Api.Entity;
using Api.Model;
using Api.Model.Session;

namespace Api.Mappers;

// The Mapper class is responsible for mapping between the Entity and the Dto.
// This class is a singleton for performance reasons, it is stateless and can be used in multiple threads.
public interface ISessionMapper
{
    SessionResponse Map(SessionEntity entity);
    SessionEntity Map(SessionRequest dto);
    PagedList<SessionResponse> Map(PagedList<SessionEntity> entities);
}

public class SessionMapper : ISessionMapper
{
    public SessionResponse Map(SessionEntity entity)
    {
        return new SessionResponse
        {
            Id = entity.Id,
            DateAdded = entity.DateAdded,
            Tags = entity.Tags?.ToList(),
            DurationSeconds = (int)entity.Duration.TotalSeconds,
            Name = entity.Name,
        };
    }

    public SessionEntity Map(SessionRequest dto)
    {
        return new SessionEntity
        {
            Tags = dto.Tags?.ToHashSet(StringComparer.OrdinalIgnoreCase),
            Duration = TimeSpan.FromSeconds(dto.DurationSeconds),
            Name = dto.Name,
        };
    }

    public PagedList<SessionResponse> Map(PagedList<SessionEntity> entities)
    {
        return new PagedList<SessionResponse>
        {
            Items = entities.Items.ConvertAll(Map),
            Total = entities.Total,
            Page = entities.Page,
            PageSize = entities.PageSize,
            Next = entities.Next
        };
    }
}
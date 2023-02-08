using Api.Entity;
using Api.Model;
using Api.Model.Session;

namespace Api.Repository.Session;

// This interface defines the contract for the session repository.
public interface ISessionRepository
{
    Task<SessionEntity?> GetAsync(string id);
    Task<PagedList<SessionEntity>> GetAllAsync(Paging paging, string? sessionName, string? tag);
    Task<SessionEntity> AddAsync(SessionEntity entity);
    Task<SessionEntity> UpdateAsync(SessionEntity entity, string etag);
    Task DeleteAsync(string sessionId, string etag);
    Task<SessionEntity?> GetByNameAsync(string name);
    Task<int> GetAverageDurationAsync(DateTimeOffset fromDateAdded, DateTimeOffset toDateAdded);
}
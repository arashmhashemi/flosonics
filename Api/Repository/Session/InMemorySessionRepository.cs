using System.Collections.Concurrent;
using Api.Entity;
using Api.Model;

namespace Api.Repository.Session;

// This is a simple in-memory repository for demo purposes.
public class InMemorySessionRepository : ISessionRepository
{
    // At the moment we won't need a concurrent dictionary as we lock on modifications.
    // But it is a good practice to use a concurrent dictionary for thread safety of future functionality.    
    private readonly ConcurrentDictionary<string, SessionEntity> _sessions = new();

    public Task<SessionEntity?> GetAsync(string id)
    {
        return Task.FromResult(_sessions.TryGetValue(id, out var session) ? session : null);
    }

    public Task<PagedList<SessionEntity>> GetAllAsync(Paging paging, string? sessionName, string? tag)
    {
        var filteredSessions = _sessions.Values;
        filteredSessions = PerformSearch(sessionName, tag, filteredSessions);

        int offset = paging.Page * paging.PageSize;

        // If the offset is greater than the total number of items, return an empty collection.
        if (offset > filteredSessions.Count)
        {
            return Task.FromResult(new PagedList<SessionEntity>());
        }

        var pagedResult = new PagedList<SessionEntity>
        {
            Page = paging.Page,
            PageSize = paging.PageSize,
            Total = filteredSessions.Count,
            Items = filteredSessions.Skip(offset).Take(paging.PageSize).ToList()
        };

        return Task.FromResult(pagedResult);
    }

    private static ICollection<SessionEntity> PerformSearch(string? sessionName, string? tag, ICollection<SessionEntity> sessions)
    {
        if (!string.IsNullOrWhiteSpace(sessionName))
        {
            sessions = sessions.Where(s => s.Name.Contains(sessionName, StringComparison.OrdinalIgnoreCase)).ToList();
        }
        if (!string.IsNullOrWhiteSpace(tag))
        {
            sessions = sessions.Where(s => s.Tags?.Contains(tag, StringComparer.OrdinalIgnoreCase) ?? false).ToList();
        }

        return sessions.OrderByDescending(s => s.DateAdded).ToList();
    }

    public Task<SessionEntity> AddAsync(SessionEntity session)
    {
        session.Id = Guid.NewGuid().ToString();
        session.DateAdded = DateTimeOffset.UtcNow;
        session.ETag = Guid.NewGuid().ToString();

        _sessions[session.Id] = session;
        return Task.FromResult(session);
    }

    public Task<SessionEntity> UpdateAsync(SessionEntity session, string etag)
    {
        _sessions.EtagThreadSafe(
            session.Id,
            etag,
            existingSession =>
        {
            existingSession.ETag = Guid.NewGuid().ToString();
            existingSession.Name = session.Name;
            existingSession.Tags = session.Tags;
            existingSession.Duration = session.Duration;
        });

        return Task.FromResult(_sessions[session.Id]);
    }

    public Task DeleteAsync(string sessionId, string sessionEtag)
    {
        _sessions.EtagThreadSafe(
            sessionId,
            sessionEtag,
            existingSession => _sessions.TryRemove(existingSession.Id, out SessionEntity? _),
            throwIfNotFound: false);

        return Task.CompletedTask;
    }

    public Task<SessionEntity?> GetByNameAsync(string name)
    {
        // TODO: Use a better data structure for searching, like a hashset.
        return Task.FromResult(_sessions.Values.FirstOrDefault(s => s.Name.Equals(name, StringComparison.OrdinalIgnoreCase)));
    }

    public Task<int> GetAverageDurationAsync(DateTimeOffset fromDateAdded, DateTimeOffset toDateAdded)
    {
        return Task.FromResult(
            _sessions.Values.Any()
            ? (int)_sessions.Values.Where(s => s.DateAdded >= fromDateAdded && s.DateAdded <= toDateAdded)
                .Average(s => s.Duration.TotalSeconds)
            : 0);
    }
}
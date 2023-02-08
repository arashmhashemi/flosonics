using System.Collections.Concurrent;
using Api.Entity;
using Api.Exceptions;

namespace Api.Model.Extensions;

public static class ThreadSafe
{
    // Thread-safe way to update an entity with etag.
    public static void EtagThreadSafe<TKey, TValue>(
        this ConcurrentDictionary<TKey, TValue> repo,
        TKey id,
        string etag,
        Action<TValue> changeAction,
        bool throwIfNotFound = true)
        where TKey : notnull
        where TValue : IEntityWithETag
    {
        // check if session exists
        if (!repo.TryGetValue(id, out var existingSession))
        {
            if (throwIfNotFound)
            {
                throw new EntityNotFoundException();
            }

            // if not found and not throwing, just return. This is useful for delete operations.
            return;
        }

        // check if session's etag matches
        // this is only needed for performance reasons to avoid locking if etag doesn't match
        if (existingSession.ETag != etag)
        {
            throw new EntityModifiedException();
        }

        lock (existingSession!) // existingSession is not null here
        {
            // double check that session's etag still matches after locking as it could have changed in the meantime
            if (existingSession.ETag != etag)
            {
                throw new EntityModifiedException();
            }

            // perform the change
            changeAction(existingSession);
        }
    }
}
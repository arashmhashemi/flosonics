namespace Api.Entity;

public class SessionEntity : IEntityWithETag
{
    public string Id { get; set; } = string.Empty;
    public string ETag { get; set; } = string.Empty;
    public DateTimeOffset DateAdded { get; set; }
    public HashSet<string>? Tags { get; set; }
    public TimeSpan Duration { get; set; }
    public string Name { get; set; } = string.Empty;
}
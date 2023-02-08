namespace Api.Model.Session;

public class SessionResponse
{
    public string Id { get; init; } = string.Empty;
    public DateTimeOffset DateAdded { get; init; }
    public List<string>? Tags { get; set; }
    public int DurationSeconds { get; set; }
    public string Name { get; set; } = string.Empty;
}
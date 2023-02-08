namespace Api.Model.Session;

public class SessionRequest
{
    public List<string>? Tags { get; set; }
    public int DurationSeconds { get; set; }
    public string Name { get; set; } = string.Empty;
}
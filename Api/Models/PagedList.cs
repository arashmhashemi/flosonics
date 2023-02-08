namespace Api.Model;

// This class is used to return a paged list of items.
public class PagedList<T>
{
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int Total { get; set; }
    public string? Next { get; set; }

    public List<T> Items { get; set; } = new();
}
namespace Api.Model;

// This class is used for paging.
public class Paging
{
    private const int _maxPageSize = 100;

    private int _page;
    public int Page
    {
        get { return _page; }
        set
        {
            _page = Math.Max(0, value); // Prevent negative values from being set.
        }
    }
    private int _pageSize;

    public int PageSize
    {
        get { return _pageSize; }
        set
        {
            _pageSize = Math.Max(1, value); // Prevent negative values and 0 from being set.

            if (_pageSize > _maxPageSize)
            {
                _pageSize = _maxPageSize;
            }
        }
    }
    public int Total { get; set; }
}
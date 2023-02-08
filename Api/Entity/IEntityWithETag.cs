namespace Api.Entity
{
    public interface IEntityWithETag
    {
        string ETag { get; set; }
    }
}
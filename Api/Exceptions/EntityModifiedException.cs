namespace Api.Exceptions;

// This exception is thrown when an entity is modified by another user.
public class EntityModifiedException : Exception
{
    public EntityModifiedException() : base("The entity has been modified by another user.")
    {
    }

    public EntityModifiedException(string? message) : base(message)
    {
    }

    public EntityModifiedException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}
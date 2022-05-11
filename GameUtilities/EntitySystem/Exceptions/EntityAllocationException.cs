namespace GameUtilities.EntitySystem.Exceptions;

public class EntityAllocationException : Exception
{
    public EntityAllocationException(string message) : base(message)
    {
    }
}
namespace GameUtilities.System.Serialization.Parsers;

public class ParseResult
{
    public ParseResult(Type componentType, object? component)
    {
        ComponentType = componentType;
        Component = component;
    }

    public ParseResult(Type componentType)
    {
        ComponentType = componentType;
    }

    public Type ComponentType { get; }
    public object? Component { get; }

    public bool Succeeded => Component != null;
}
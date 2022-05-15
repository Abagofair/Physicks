using System.Text.Json;

namespace GameUtilities.System.Serialization.Parsers;

public interface IComponentParser
{
    abstract Type ComponentType { get; }
    abstract List<IPropertyParser> PropertyParsers { get; }
    ParseResult Parse(ref Utf8JsonReader jsonReader);
}
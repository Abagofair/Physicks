using System.Text.Json;
using GameUtilities.System.Serialization.PropertyParsers;

namespace GameUtilities.System.Serialization.ComponentParsers;

public interface IComponentParser
{
    abstract Type ComponentType { get; }
    abstract Dictionary<Type, IPropertyParser> PropertyParsers { get; }
    ParseResult Parse(ref Utf8JsonReader jsonReader);
}
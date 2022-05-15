using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace GameUtilities.System.Serialization.Parsers;

public abstract class ComponentParser : IComponentParser
{
    private readonly Dictionary<string, PropertyInfo> _serializableProperties;

    public ComponentParser()
    {
        _serializableProperties = ComponentType
            .GetProperties()
            .Where(x => x.CustomAttributes.Any(a => a.AttributeType == typeof(JsonIncludeAttribute)))
            .ToDictionary(x => x.Name);
    }

    public abstract Type ComponentType { get; }

    public abstract List<IPropertyParser> PropertyParsers { get; }

    public ParseResult Parse(ref Utf8JsonReader jsonReader)
    {
        if (jsonReader.TokenType != JsonTokenType.StartObject)
            throw new SerializationException(@$"Expected JsonTokenType.StartObject got JsonTokenType.{jsonReader.TokenType}");

        object? component = Activator.CreateInstance(ComponentType);
        if (component == null)
            return new ParseResult(ComponentType);

        var stack = new Stack<JsonTokenType>();
        stack.Push(JsonTokenType.StartObject);

        //todo need to handle finishing an entire component at EndObject
        //or just find a better way to parse an object
        while (jsonReader.Read() && stack.Count > 0)
        {
            switch (jsonReader.TokenType)
            {
                case JsonTokenType.StartObject:
                    {
                        stack.Push(JsonTokenType.StartObject);
                        break;
                    }
                case JsonTokenType.EndObject:
                    {
                        stack.Pop();
                        break;
                    }
                case JsonTokenType.PropertyName:
                    {
                        string? key = jsonReader.GetString();
                         if (key != null &&
                            _serializableProperties.TryGetValue(key, out PropertyInfo? propertyInfo))
                        {
                            foreach (IPropertyParser propertyParser in PropertyParsers)
                            {
                                if (propertyParser.Type == propertyInfo.PropertyType)
                                {
                                    propertyParser.SetValue(ref jsonReader, propertyInfo, component);
                                }
                                else if (propertyParser.Type.GetInterface(propertyInfo.PropertyType.Name) != null)
                                {
                                    propertyParser.SetValue(ref jsonReader, propertyInfo, component);
                                }
                            }
                        }
                        break;
                    }
            }
        }

        return new ParseResult(ComponentType, component);
    }
}
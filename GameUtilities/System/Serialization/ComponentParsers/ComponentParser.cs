using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using GameUtilities.System.Serialization.PropertyParsers;

namespace GameUtilities.System.Serialization.ComponentParsers;

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

    public abstract Dictionary<Type, IPropertyParser> PropertyParsers { get; }

    public ParseResult Parse(ref Utf8JsonReader jsonReader)
    {
        object? component = Activator.CreateInstance(ComponentType);
        if (component == null)
            return new ParseResult(ComponentType);

        //todo need to handle finishing an entire component at EndObject
        //or just find a better way to parse an object
        while (jsonReader.Read())
        {
            switch (jsonReader.TokenType)
            {
                case JsonTokenType.StartObject:
                    {
                        continue;
                    }
                case JsonTokenType.PropertyName:
                    {
                        string? key = jsonReader.GetString();
                        if (key != null && _serializableProperties.TryGetValue(key, out PropertyInfo? propertyInfo))
                        {
                            if (PropertyParsers.TryGetValue(propertyInfo.PropertyType, out IPropertyParser? parser))
                            {
                                parser.SetValue(ref jsonReader, propertyInfo, component);

                               /* while (jsonReader.TokenType != JsonTokenType.StartObject &&
                                    jsonReader.TokenType != JsonTokenType.PropertyName &&
                                    jsonReader.TokenType != JsonTokenType.StartArray)
                                {
                                    jsonReader.Read();
                                }*/
                            }
                        }
                        break;
                    }
            }
        }

        return new ParseResult(ComponentType, component);
    }
}
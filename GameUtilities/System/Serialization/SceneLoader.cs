using System.Text.Json;
using GameUtilities.EntitySystem;
using GameUtilities.Scene;
using GameUtilities.System.Serialization.Parsers;

namespace GameUtilities.System.Serialization;

public class SceneLoader
{
    private readonly static string EntitiesPropertyName = "Entities";
    private static int EntityCount = 0;

    private JsonReaderOptions _options = new JsonReaderOptions()
    {
        AllowTrailingCommas = true,
        CommentHandling = JsonCommentHandling.Skip
    };

    private readonly Dictionary<string, IComponentParser> _componentParsers;

    public SceneLoader(params IComponentParser[] componentTypes)
    {
        _componentParsers = new Dictionary<string, IComponentParser>();
        foreach (IComponentParser componentParser in componentTypes)
        {
            _componentParsers.TryAdd(componentParser.ComponentType.Name, componentParser);
        }
    }

    public SceneGraph Load(string fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName)) throw new ArgumentNullException(nameof(fileName));
        if (!File.Exists(fileName)) throw new ArgumentException(nameof(fileName));

        var sceneGraph = new SceneGraph();
        var jsonReader = new Utf8JsonReader(File.ReadAllBytes(fileName), _options);
        while (jsonReader.Read())
        {
            switch (jsonReader.TokenType)
            {
                case JsonTokenType.StartObject:
                    continue;
                case JsonTokenType.EndObject:
                    continue;
                case JsonTokenType.PropertyName:
                    {
                        var name = jsonReader.GetString();
                        jsonReader.Read();

                        if (name == EntitiesPropertyName)
                        {
                            sceneGraph.AddEntities(ParseEntities(ref jsonReader));
                        }
                        break;
                    }
            }
        }

        return sceneGraph;
    }

    private List<EntityContext> ParseEntities(ref Utf8JsonReader jsonReader)
    {
        if (jsonReader.TokenType != JsonTokenType.StartArray)
            throw new SerializationException(@$"Expected JsonTokenType.StartArray got JsonTokenType.{jsonReader.TokenType}");

        var list = new List<EntityContext>();

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
                        list.Add(ParseEntity(ref jsonReader));
                        break;
                    }
            }
        }

        return list;
    }

    private EntityContext ParseEntity(ref Utf8JsonReader jsonReader)
    {
        jsonReader.Read();

        string? entityName = jsonReader.GetString();
        jsonReader.Read();

        string? componentsPropertyName = jsonReader.GetString();
        jsonReader.Read();

        var entityContext = new EntityContext(entityName ?? $"Entity_{EntityCount++}");

        if (componentsPropertyName == "Components" && jsonReader.TokenType == JsonTokenType.StartArray)
        {
            //todo need to handle finishing an entire component at EndObject
            //or just find a better way to parse an object
            while (jsonReader.Read() && jsonReader.TokenType != JsonTokenType.EndArray)
            {
                switch (jsonReader.TokenType)
                {
                    case JsonTokenType.StartObject:
                        {
                            var parseResult = ParseComponent(ref jsonReader);
                            if (parseResult?.Succeeded == true)
                            {
                                entityContext.AddOrOverride(parseResult.ComponentType, parseResult.Component);
                            }

                            break;
                        }
                    case JsonTokenType.EndObject:
                        {
                            break;
                        }
                }
            }
        }

        return entityContext;
    }

    private ParseResult? ParseComponent(ref Utf8JsonReader jsonReader)
    {
        jsonReader.Read();
        string componentName = jsonReader.GetString() ?? string.Empty;
        jsonReader.Read();

        if (_componentParsers.TryGetValue(componentName, out IComponentParser componentParser))
        {
            return componentParser.Parse(ref jsonReader);
        }

        return null;
    }
}
using System.Text.Json;
using GameUtilities.EntitySystem;
using GameUtilities.Scene;
using GameUtilities.System.Serialization.ComponentParsers;

namespace GameUtilities.System.Serialization;

public class SceneLoader
{
    private readonly static string EntitiesPropertyName = "Entities";

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
                            sceneGraph.AddEntity(ParseEntity(ref jsonReader));
                        }
                        break;
                    }
            }
        }

        return sceneGraph;
    }

    private EntityContext ParseEntity(ref Utf8JsonReader jsonReader)
    {
        jsonReader.Read();
        jsonReader.Read();
        jsonReader.Read();

        string? entityName = jsonReader.GetString();
        jsonReader.Read();

        string? componentsPropertyName = jsonReader.GetString();
        jsonReader.Read();

        var entityContext = new EntityContext();

        if (componentsPropertyName == "Components" && jsonReader.TokenType == JsonTokenType.StartArray)
        {
            var parseResult = ParseComponent(ref jsonReader);
            if (parseResult?.Succeeded == true)
            {
                entityContext.AddOrOverride(parseResult.ComponentType, parseResult.Component);
            }
        }

        return entityContext;
    }

    private ParseResult? ParseComponent(ref Utf8JsonReader jsonReader)
    {
        jsonReader.Read();
        jsonReader.Read();
        string componentName = jsonReader.GetString() ?? string.Empty;

        if (_componentParsers.TryGetValue(componentName, out IComponentParser componentParser))
        {
            return componentParser.Parse(ref jsonReader);
        }

        return null;
    }
}
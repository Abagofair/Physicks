using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using GameUtilities.Scene;

namespace GameUtilities.System.Serialization;

public class SceneLoader
{
    private readonly static string EntitiesPropertyName = "Entities";

    private JsonReaderOptions _options = new JsonReaderOptions()
    {
        AllowTrailingCommas = true,
        CommentHandling = JsonCommentHandling.Skip
    };

    private readonly Dictionary<string, Type> _typesByComponentName;

    public SceneLoader(params Type[] componentTypes)
    {
        _typesByComponentName = new Dictionary<string, Type>();
        foreach (var componentType in componentTypes)
        {
            _typesByComponentName.TryAdd(componentType.Name, componentType);
        }
    }

    public SceneGraph Load(string fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName)) throw new ArgumentNullException(nameof(fileName));
        if (!File.Exists(fileName)) throw new ArgumentException(nameof(fileName));

        using var fileStream = File.OpenRead(fileName);

        var count = 0;
        using var memoryStream = new MemoryStream();
        while (fileStream.Position < fileStream.Length)
        {
            var buffer = new byte[4096];
            fileStream.Read(buffer, 0, buffer.Length);
            memoryStream.Write(buffer, count, buffer.Length);
            count += 4096;
        }

        var jsonReader = new Utf8JsonReader(memoryStream.GetBuffer(), _options);
        while (jsonReader.Read())
        {
            switch (jsonReader.TokenType)
            {
                case JsonTokenType.PropertyName:
                {
                    var name = jsonReader.GetString();
                    jsonReader.Read();
                    if (name == EntitiesPropertyName && jsonReader.TokenType == JsonTokenType.StartArray)
                    {
                        ParseEntity(ref jsonReader);
                    }
                    break;
                }
            }
        }

        return new SceneGraph();
    }

    private void ParseEntity(ref Utf8JsonReader jsonReader)
    {
        jsonReader.Read();
        jsonReader.Read();
        jsonReader.Read();
        string? entityName = jsonReader.GetString();

        jsonReader.Read();

        string? componentsPropertyName = jsonReader.GetString();
        jsonReader.Read();

        if (componentsPropertyName == "Components" && jsonReader.TokenType == JsonTokenType.StartArray)
        {
            ParseComponent(ref jsonReader);
        }
    }

    private void ParseComponent(ref Utf8JsonReader jsonReader)
    {
        //todo: somehow loop over the remaining component objects and properties and correctly map that to a type in the component type map
        jsonReader.Read();
        jsonReader.Read();
        string componentName = jsonReader.GetString() ?? string.Empty;

        if (_typesByComponentName.TryGetValue(componentName, out var type))
        {
            var component = Activator.CreateInstance(type);
            if (component == null)
                return;

            IEnumerable<PropertyInfo> serializableProperties = component
                .GetType()
                .GetProperties()
                .Where(x => x.CustomAttributes.Any(a => a.AttributeType == typeof(JsonIncludeAttribute)));


        }
    }
}
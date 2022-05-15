using System.Reflection;
using System.Text.Json;
using Physicks.Collision;

namespace GameUtilities.System.Serialization.Parsers.Physicks;

public class BoxShapePropertyParser : IPropertyParser
{
    public Type Type => typeof(BoxShape);

    public void SetValue(ref Utf8JsonReader jsonReader, PropertyInfo propertyInfo, object setValueObject)
    {
        jsonReader.Read(); //consume propertyName
        jsonReader.Read(); //consume startObject

        var width = jsonReader.GetString();
        jsonReader.Read(); //consume propertyName
        if (width?.ToLowerInvariant() != "width")
            throw new JsonException(nameof(width));
        float parsedWidth = (float)jsonReader.GetDouble();

        jsonReader.Read(); //consume number
        var height = jsonReader.GetString();
        jsonReader.Read(); //consume propertyName
        if (height?.ToLowerInvariant() != "height")
            throw new JsonException(nameof(width));
        float parsedHeight = (float)jsonReader.GetDouble();

        propertyInfo.SetValue(setValueObject, new BoxShape(parsedWidth, parsedHeight));
    }
}
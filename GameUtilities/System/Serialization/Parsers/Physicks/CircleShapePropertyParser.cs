using System.Reflection;
using System.Text.Json;
using Physicks.Collision;

namespace GameUtilities.System.Serialization.Parsers.Physicks;

public class CircleShapePropertyParser : IPropertyParser
{
    public Type Type => typeof(CircleShape);

    public void SetValue(ref Utf8JsonReader jsonReader, PropertyInfo propertyInfo, object setValueObject)
    {
        jsonReader.Read(); //consume propertyName
        jsonReader.Read(); //consume startObject

        var radius = jsonReader.GetString();
        jsonReader.Read(); //consume propertyName
        if (radius?.ToLowerInvariant() != "radius")
            throw new JsonException(nameof(radius));
        float parsedRadius = (float)jsonReader.GetDouble();

        propertyInfo.SetValue(setValueObject, new CircleShape(parsedRadius));
    }
}
using System.Numerics;
using System.Reflection;
using System.Text.Json;

namespace GameUtilities.System.Serialization.Parsers;

public class Vector2PropertyParser : IPropertyParser
{
    public Type Type => typeof(Vector2);

    public void SetValue(
        ref Utf8JsonReader jsonReader,
        PropertyInfo propertyInfo,
        object setValueObject)
    {
        jsonReader.Read(); //consume propertyName
        jsonReader.Read(); //consume startObject

        var xName = jsonReader.GetString();
        jsonReader.Read(); //consume propertyName
        if (xName != "x")
            throw new JsonException(nameof(xName));
        float xValue = (float)jsonReader.GetDouble();

        jsonReader.Read(); //consume number
        var yName = jsonReader.GetString();
        jsonReader.Read(); //consume propertyName
        if (yName != "y")
            throw new JsonException(nameof(xName));
        float yValue = (float)jsonReader.GetDouble();

        propertyInfo.SetValue(setValueObject, new Vector2(xValue, yValue));

        jsonReader.Read();
    }
}
using System.Reflection;
using System.Text.Json;

namespace GameUtilities.System.Serialization.Parsers;

public class FloatPropertyParser : IPropertyParser
{
    public Type Type => typeof(float);

    public void SetValue(ref Utf8JsonReader jsonReader, PropertyInfo propertyInfo, object setValueObject)
    {
        jsonReader.Read();
        float value = (float)jsonReader.GetDouble();
        propertyInfo.SetValue(setValueObject, value);
    }
}
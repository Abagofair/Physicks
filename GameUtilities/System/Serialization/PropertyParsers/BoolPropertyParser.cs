using System.Reflection;
using System.Text.Json;

namespace GameUtilities.System.Serialization.PropertyParsers;

public class BoolPropertyParser : IPropertyParser
{
    public Type Type => typeof(bool);

    public void SetValue(ref Utf8JsonReader jsonReader, PropertyInfo propertyInfo, object setValueObject)
    {
        jsonReader.Read();
        bool value = jsonReader.GetBoolean();
        propertyInfo.SetValue(setValueObject, value);
    }
}
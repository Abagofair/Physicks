using System.Reflection;
using System.Text.Json;

namespace GameUtilities.System.Serialization.Parsers;

public interface IPropertyParser
{
    Type Type { get; }

    void SetValue(
        ref Utf8JsonReader jsonReader,
        PropertyInfo propertyInfo,
        object setValueObject);
}
using System.Numerics;
using GameUtilities.System.Serialization.ComponentParsers;
using GameUtilities.System.Serialization.PropertyParsers;

namespace Physicks.Serialization;

public class BodyParser : ComponentParser
{
    public BodyParser()
    {
        PropertyParsers = new Dictionary<Type, IPropertyParser>
        {
            { typeof(float), new FloatPropertyParser() },
            { typeof(bool), new BoolPropertyParser() },
            { typeof(Vector2), new Vector2PropertyParser() }
        };
    }

    public override Type ComponentType => typeof(Body);

    public override Dictionary<Type, IPropertyParser> PropertyParsers { get; }
}
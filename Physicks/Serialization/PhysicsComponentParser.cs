using GameUtilities.System.Serialization.ComponentParsers;
using GameUtilities.System.Serialization.PropertyParsers;

namespace Physicks.Serialization;

public class PhysicsComponentParser : ComponentParser
{
    public PhysicsComponentParser()
    {
        PropertyParsers = new Dictionary<Type, IPropertyParser>
        {
            { typeof(float), new FloatPropertyParser() },
            { typeof(bool), new BoolPropertyParser() },
            { typeof(Vector2PropertyParser), new Vector2PropertyParser() }
        };
    }

    public override Type ComponentType => typeof(PhysicsComponent);

    public override Dictionary<Type, IPropertyParser> PropertyParsers { get; }
}
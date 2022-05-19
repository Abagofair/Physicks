using GameUtilities.System.Serialization.Parsers;
using GameUtilities.System.Serialization.Parsers.Physicks;
using Physicks;
using Physicks.Collision;

namespace GameUtilities.Serialization.Parsers.Physicks;

public class BodyComponentParser : ComponentParser
{
    public BodyComponentParser()
        : base(typeof(BoxShape), typeof(CircleShape), typeof(PolygonShape)) //TODO: Should be overridable property?
    {
        PropertyParsers = new List<IPropertyParser>
        {
            new FloatPropertyParser(),
            new BoolPropertyParser(),
            new Vector2PropertyParser(),
            new BoxShapePropertyParser(),
            new CircleShapePropertyParser()
        };
    }

    public override Type ComponentType => typeof(Body);

    public override List<IPropertyParser> PropertyParsers { get; }
}
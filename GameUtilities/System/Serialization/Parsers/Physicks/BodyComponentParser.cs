using GameUtilities.System.Serialization.Parsers;
using GameUtilities.System.Serialization.Parsers.Physicks;
using Physicks;

namespace GameUtilities.Serialization.Parsers.Physicks;

public class BodyComponentParser : ComponentParser
{
    public BodyComponentParser()
    {
        PropertyParsers = new List<IPropertyParser>
        {
            new FloatPropertyParser(),
            new BoolPropertyParser(),
            new Vector2PropertyParser(),
            new BoxShapePropertyParser()
        };
    }

    public override Type ComponentType => typeof(Body);

    public override List<IPropertyParser> PropertyParsers { get; }
}
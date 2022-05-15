using GameUtilities.System.Serialization.Parsers;
using MonoGameUtilities.Rendering;

namespace MonoGameUtilities.Serialization;

public class RenderableQuadComponentParser : ComponentParser
{
    public RenderableQuadComponentParser()
    {
        PropertyParsers = new List<IPropertyParser>
        {
            new BoolPropertyParser(),
            new Vector2PropertyParser()
        };
    }

    public override Type ComponentType => typeof(RenderableQuad);

    public override List<IPropertyParser> PropertyParsers { get; }
}
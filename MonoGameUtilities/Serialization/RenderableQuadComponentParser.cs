using GameUtilities.System.Serialization.ComponentParsers;
using GameUtilities.System.Serialization.PropertyParsers;
using MonoGameUtilities.Rendering;

namespace MonoGameUtilities.Serialization;

public class RenderableQuadComponentParser : ComponentParser
{
    public RenderableQuadComponentParser()
    {
        PropertyParsers = new Dictionary<Type, IPropertyParser>
        {
            { typeof(bool), new BoolPropertyParser() }
        };
    }

    public override Type ComponentType => typeof(RenderableQuadComponent);

    public override Dictionary<Type, IPropertyParser> PropertyParsers { get; }
}
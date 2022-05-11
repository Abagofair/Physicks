using Microsoft.Xna.Framework;

namespace MonoGameUtilities.Rendering;
public interface IRenderer
{
    void Draw(RenderableQuad renderable, Matrix? transform = null);
}

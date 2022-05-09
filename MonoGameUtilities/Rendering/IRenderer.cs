using Microsoft.Xna.Framework;

namespace MonoGameUtilities.Rendering;
public interface IRenderer
{
    void Draw(RenderableQuadComponent renderable, Matrix? transform = null);
}

using Microsoft.Xna.Framework;

namespace MonoGameUtilities.Rendering;
public interface IRenderer
{
    void Draw(RenderableSpriteComponent renderable, Matrix? transform = null);
}

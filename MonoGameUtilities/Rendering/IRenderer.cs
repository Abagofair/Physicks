using Microsoft.Xna.Framework;

namespace MonoGameUtilities.Rendering;
public interface IRenderer
{
    void Draw(Renderable renderable, Matrix? transform = null);
}

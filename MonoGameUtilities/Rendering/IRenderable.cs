using Microsoft.Xna.Framework.Graphics;

namespace MonoGameUtilities.Rendering;

public interface IRenderable
{
    bool IsDrawable { get; set; }
    VertexBuffer? VertexBuffer { get; }
    IndexBuffer? IndexBuffer { get; }
    void SetupBuffers(GraphicsDevice graphicsDevice);
}

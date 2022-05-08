using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGameUtilities.Rendering;

public class DebugSpriteRenderer : IRenderer
{
    private readonly SpriteEffect _spriteEffect;
    private readonly GraphicsDevice _graphicsDevice;

    public DebugSpriteRenderer(GraphicsDevice graphicsDevice)
    {
        _graphicsDevice = graphicsDevice ?? throw new ArgumentNullException(nameof(graphicsDevice));
        _spriteEffect = new SpriteEffect(graphicsDevice);
    }

    public void Draw(RenderableSpriteComponent renderable, Matrix? transform = null)
    {
        if (renderable == null) return;
        if (transform == null) transform = Matrix.Identity;

        SetRenderState();

        _graphicsDevice.SetVertexBuffer(renderable.VertexBuffer);

        _spriteEffect.TransformMatrix = transform;
        _spriteEffect.CurrentTechnique.Passes[0].Apply();

        _graphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, renderable.Vertices.Length / 3);
        //_graphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, vertices, 0, vertices.Length, indices, 0, 1, new VertexDeclaration(
        //        new VertexElement(0, VertexElementFormat.Vector2, VertexElementUsage.Position, 0)));
    }

    private void SetRenderState()
    {
        _graphicsDevice.RasterizerState = new RasterizerState()
        {
            FillMode = FillMode.WireFrame
        };
    }
}

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGameUtilities.Rendering;

public class SpriteRenderer : IRenderer
{
    private readonly GraphicsDevice _graphicsDevice;
    private readonly SpriteEffect _spriteEffect;

    public SpriteRenderer(GraphicsDevice graphicsDevice)
    {
        _graphicsDevice = graphicsDevice ?? throw new ArgumentNullException(nameof(graphicsDevice));
        _spriteEffect = new SpriteEffect(graphicsDevice);
    }

    public void Draw(RenderableSpriteComponent renderable, Matrix? transform)
    {
        if (renderable == null) return;
        if (transform == null) transform = Matrix.Identity;

        SetRenderState();

        _graphicsDevice.SetVertexBuffer(renderable.VertexBuffer);

        _spriteEffect.TransformMatrix = transform;
        _spriteEffect.CurrentTechnique.Passes[0].Apply();

        _graphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, renderable.Vertices.Length / 3);
    }

    private void SetRenderState()
    {
        _graphicsDevice.RasterizerState = new RasterizerState()
        {
            FillMode = FillMode.Solid
        };

        _graphicsDevice.BlendState = BlendState.AlphaBlend;
    }
}
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

    public void Draw(RenderableQuadComponent renderable, Matrix? transform = null)
    {
        if (renderable == null) return;
        if (!renderable.IsSetup) return;
        if (transform == null) transform = Matrix.Identity;

        SetRenderState();

        _graphicsDevice.SetVertexBuffer(renderable.VertexBuffer);
        _graphicsDevice.Indices = renderable.IndexBuffer;
        _spriteEffect.TransformMatrix = transform;

        foreach (var item in _spriteEffect.CurrentTechnique.Passes)
        {
            item.Apply();
            _graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, 2);
            //_graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, RenderPrimitives.QuadVertices, 0, 1, Vertex.VertexDeclaration);
        }

        //_graphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, vertices, 0, vertices.Length, indices, 0, 1, new VertexDeclaration(
        //        new VertexElement(0, VertexElementFormat.Vector2, VertexElementUsage.Position, 0)));
    }

    private void SetRenderState()
    {
        _graphicsDevice.RasterizerState = new RasterizerState()
        {
            FillMode = FillMode.Solid,
            CullMode = CullMode.CullClockwiseFace
        };

       // _graphicsDevice.SamplerStates[0] = SamplerState.PointWrap;

        //_graphicsDevice.DepthStencilState = DepthStencilState.Default;

        //_graphicsDevice.BlendState = BlendState.Opaque;
    }
}

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGameUtilities.Rendering;

public class DebugSpriteRenderer
{
    private SpriteEffect _spriteEffect;
    private GraphicsDevice _graphicsDevice;

    public DebugSpriteRenderer(GraphicsDevice graphicsDevice)
    {
        _graphicsDevice = graphicsDevice ?? throw new ArgumentNullException(nameof(graphicsDevice));
        _spriteEffect = new SpriteEffect(graphicsDevice);
    }

    public void Draw(Vector2[] vertices, Matrix? transform = null)
    {
        if (vertices == null || vertices.Length < 3) return;
        if (transform == null) transform = Matrix.Identity;

        var vb = new VertexBuffer(
            _graphicsDevice,
            new VertexDeclaration(
                new VertexElement(0, VertexElementFormat.Vector2, VertexElementUsage.Position, 0)),
            vertices.Length,
            BufferUsage.None);

        vb.SetData(vertices);
        _graphicsDevice.RasterizerState = new RasterizerState()
        {
            FillMode = FillMode.WireFrame
        };
        _graphicsDevice.SetVertexBuffer(vb);
        _spriteEffect.TransformMatrix = transform;
        _spriteEffect.CurrentTechnique.Passes[0].Apply();
        _graphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, vertices.Length / 3);
        //_graphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, vertices, 0, vertices.Length, indices, 0, 1, new VertexDeclaration(
        //        new VertexElement(0, VertexElementFormat.Vector2, VertexElementUsage.Position, 0)));
    }
}

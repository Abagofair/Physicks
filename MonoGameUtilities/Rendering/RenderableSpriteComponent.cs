using Microsoft.Xna.Framework.Graphics;

namespace MonoGameUtilities.Rendering;

public class RenderableSpriteComponent
{
    public RenderableSpriteComponent(
        GraphicsDevice graphicsDevice,
        Vertex[] vertices)
    {
        Vertices = vertices ?? throw new ArgumentNullException(nameof(vertices));

        VertexBuffer = new VertexBuffer(
            graphicsDevice,
            Vertex.VertexDeclaration,
            vertices.Length,
            BufferUsage.None);

        VertexBuffer.SetData(0, vertices, 0, Vertices.Length, Vertex.VertexDeclaration.VertexStride);
    }

    public bool IsDrawable { get; set; } = true;
    public Vertex[] Vertices { get; } //maybe not necessary to keep in memory?
    public VertexBuffer VertexBuffer { get; }
}
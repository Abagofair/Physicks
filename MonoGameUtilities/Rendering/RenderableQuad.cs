using System.Text.Json.Serialization;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGameUtilities.Rendering;

public class RenderableQuad : IRenderable
{
    private bool _isSetup;

    [JsonInclude]
    public bool IsDrawable { get; set; } = true;

    public bool IsSetup => _isSetup;

    public VertexBuffer? VertexBuffer { get; private set; }

    public IndexBuffer? IndexBuffer { get; private set; }

    public void SetupBuffers(GraphicsDevice graphicsDevice)
    {
        VertexBuffer = new VertexBuffer(
            graphicsDevice,
            Vertex.VertexDeclaration,
            RenderPrimitives.QuadVertices.Length,
            BufferUsage.WriteOnly);

        /*VertexBuffer.SetData(
            0,
            RenderPrimitives.QuadVertices,
            0,
            RenderPrimitives.QuadVertices.Length,
            Vertex.VertexDeclaration.VertexStride);*/
        VertexBuffer.SetData(RenderPrimitives.QuadVertices);

        IndexBuffer = new IndexBuffer(
            graphicsDevice,
            IndexElementSize.SixteenBits,
            RenderPrimitives.QuadIndices.Length,
            BufferUsage.WriteOnly);

        IndexBuffer.SetData(RenderPrimitives.QuadIndices);

        _isSetup = true;
    }
}
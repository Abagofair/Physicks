using System.Text.Json.Serialization;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGameUtilities.Rendering;

public class RenderableQuadComponent : IRenderable
{

    [JsonInclude]
    public bool IsDrawable { get; set; } = true;

    public VertexBuffer? VertexBuffer { get; private set; }

    public IndexBuffer? IndexBuffer { get; private set; }

    public void SetupBuffers(GraphicsDevice graphicsDevice)
    {
        VertexBuffer = new VertexBuffer(
            graphicsDevice,
            Vertex.VertexDeclaration,
            RenderPrimitives.QuadVertices.Length,
            BufferUsage.None);

        VertexBuffer.SetData(
            0,
            RenderPrimitives.QuadVertices,
            0,
            RenderPrimitives.QuadVertices.Length,
            Vertex.VertexDeclaration.VertexStride);

        IndexBuffer = new IndexBuffer(
            graphicsDevice,
            IndexElementSize.ThirtyTwoBits,
            RenderPrimitives.QuadIndices.Length,
            BufferUsage.None);

        IndexBuffer.SetData(
            RenderPrimitives.QuadIndices);
    }
}
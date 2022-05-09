using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGameUtilities.Rendering;

[StructLayout(LayoutKind.Explicit)]
public struct Vertex
{
    public static readonly VertexDeclaration VertexDeclaration;

    public Vertex(Vector3 position, Vector2 textureCoordinate)
    {
        Position = position;
        TextureCoordinate = textureCoordinate;
    }

    static Vertex()
    {
        VertexDeclaration = new VertexDeclaration(
                new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
                new VertexElement(12, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0));
    }

    [FieldOffset(0)]
    public Vector3 Position;

    [FieldOffset(12)]
    public Vector2 TextureCoordinate;
}
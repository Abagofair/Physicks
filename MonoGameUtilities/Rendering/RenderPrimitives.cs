using Microsoft.Xna.Framework;

namespace MonoGameUtilities.Rendering;

public static class RenderPrimitives
{
    public static readonly Vertex[] QuadVertices = new[]
    {
        new Vertex(new Vector3(-1.0f, 1.0f, 0.0f), new Vector2(-1.0f, 1.0f)),
        new Vertex(new Vector3(1.0f, 1.0f, 0.0f), new Vector2(1.0f, 1.0f)),
        new Vertex(new Vector3(1.0f, -1.0f, 0.0f), new Vector2(1.0f, -1.0f)),
        new Vertex(new Vector3(-1.0f, -1.0f, 0.0f), new Vector2(-1.0f, -1.0f))
    };

    public static readonly int[] QuadIndices = new int[]
    {
        0, 1, 2,
        0, 2, 3
    };
}
using Microsoft.Xna.Framework;

namespace MonoGameUtilities.Rendering;

public static class RenderPrimitives
{
    public static readonly Vertex[] QuadVertices = new[]
    {
        new Vertex(new Vector3(0.5f, 0.5f, 0.0f), new Vector2(1.0f, 1.0f)),
        new Vertex(new Vector3(0.5f, -0.5f, 0.0f), new Vector2(1.0f, 0.0f)),
        new Vertex(new Vector3(-0.5f, -0.5f, 0.0f), new Vector2(0.0f, 0.0f)),
        new Vertex(new Vector3(-0.5f, 0.5f, 0.0f), new Vector2(0.0f, 1.0f))
    };

    /*public static readonly Vertex[] QuadVertices = new[]
    {
        new Vertex(new Vector3(10.5f, -10.5f, 0.0f), new Vector2(1.0f, 1.0f)),
        new Vertex(new Vector3(-10.5f, -10.5f, 0.0f), new Vector2(0.0f, 0.0f)),
        new Vertex(new Vector3(10.0f, 10.5f, 0.0f), new Vector2(0.0f, 1.0f))
    };*/

    public static readonly short[] QuadIndices = new short[]
    {
        0, 1, 3,
        1, 2, 3
    };
}
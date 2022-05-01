using GameUtilities.Triangulation;
using System.Numerics;

namespace GameUtilities.Meshes;

public class Triangulation
{
    public Triangulation()
    {
        Triangles = new List<Triangle>();
    }

    public List<Triangle> Triangles { get; set; }

    public static Triangulation FromVertices(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4)
    {
        var v1 = new Vertex(p1);
        var v2 = new Vertex(p2);
        var v3 = new Vertex(p3);
        var v4 = new Vertex(p4);

        return FromVertices(v1, v2, v3, v4);
    }

    public static Triangulation FromVertices(Vertex v1, Vertex v2, Vertex v3, Vertex v4)
    {
        var e1 = new Edge(v1, v2);
        var e2 = new Edge(v2, v3);
        var e3 = new Edge(v3, v4);
        var e4 = new Edge(v4, v1);
        var e5 = new Edge(v3, v1);

        var t1 = new Triangle(e1, e2, e5);
        var t2 = new Triangle(e5, e3, e4);

        var triangulation = new Triangulation();
        triangulation.Triangles.Add(t1);
        triangulation.Triangles.Add(t2);

        return triangulation;
    }
}


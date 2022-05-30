using System.Numerics;

namespace Physicks.Collision;

public class PolygonShape : IShape
{
    public PolygonShape()
    {
        Vertices = Array.Empty<Vector2>();
        VerticesInWorld = Array.Empty<Vector2>();
    }

    public Vector2[] Vertices { get; set; }
    public Vector2[] VerticesInWorld { get; set; }
    public virtual float MomentOfInertia { get; set; }

    public void TransformVertices(Matrix4x4 transform)
    {
        if (Vertices.Length != VerticesInWorld?.Length)
        {
            VerticesInWorld = new Vector2[Vertices.Length];
        }

        for (int i = 0; i < Vertices.Length; i++)
        {
            Vector2 vertex = Vertices[i];
            VerticesInWorld[i] = Vector2.Transform(vertex, transform);
        }
    }

    public Vector2 EdgeAt(int index)
    {
        int currVertex = index;
        int nextVertex = (index + 1) % VerticesInWorld.Length;
        return VerticesInWorld[nextVertex] - VerticesInWorld[currVertex];
    }

    public int FindIncidentEdge(Vector2 normal)
    {
        int indexIncidentEdge = 0;
        float minProj = float.MaxValue;

        for (int i = 0; i < VerticesInWorld.Length; i++)
        {
            var edgeNormal = Vector2.Normalize(new Vector2(EdgeAt(i).Y, -EdgeAt(i).X));
            var proj = Vector2.Dot(edgeNormal, normal);

            if (proj < minProj)
            {
                minProj = proj;
                indexIncidentEdge = i;
            }
        }

        return indexIncidentEdge;
    }

    public int ClipSegmentToLine(List<Vector2> contactsIn, List<Vector2> contactsOut, Vector2 c0, Vector2 c1)
    {
        int numOut = 0;

        Vector2 normal = Vector2.Normalize((c1 - c0));
        float dist0 = Body.Cross((contactsIn[0] - c0), normal);
        float dist1 = Body.Cross((contactsIn[1] - c0), normal);

        if (dist0 <= 0)
        {
            contactsOut[numOut++] = contactsIn[0];
        }
        if (dist1 <= 0)
        {
            contactsOut[numOut++] = contactsIn[1];
        }

        if (dist0 * dist1 < 0)
        {
            float totalDist = dist0 - dist1;

            float t = dist0 / (totalDist);
            Vector2 contact = contactsIn[0] + (contactsIn[1] - contactsIn[0]) * t;
            contactsOut[numOut] = contact;
            numOut++;
        }

        return numOut;
    }
}

using System.Numerics;
using Physicks.Math;

namespace Physicks.Collision;

public class PolygonShape : IShape
{
    public PolygonShape(
        Vector2[] vertices,
        float momentOfInertia,
        float mass)
    {
        if (vertices == null || vertices.Length == 0) throw new ArgumentException("Cannot be empty or null", nameof(vertices));

        Vertices = vertices;

        MomentOfInertia = momentOfInertia;
        InverseMomentOfInertia = 1.0f / momentOfInertia;
        Mass = mass;
        InverseMass = 1.0f / mass;
    }

    public PolygonShape(
        Particle[] particles,
        float momentOfInertia,
        float mass)
    {
        if (particles == null || particles.Length == 0) throw new ArgumentException("Cannot be empty or null", nameof(particles));

        Vertices = ParticleToPosition(particles).ToArray();
        
        MomentOfInertia = momentOfInertia;
        InverseMomentOfInertia = 1.0f / momentOfInertia;
        Mass = mass;
        InverseMass = 1.0f / mass;
    }

    public Vector2[] Vertices { get; set; }

    public float Mass { get; set; }
    public float InverseMass { get; set; }
    public float MomentOfInertia { get; set; }
    public float InverseMomentOfInertia { get; set; }

    /// <summary>
    /// Get edge at index in local space
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public Vector2 EdgeAt(int index, Matrix4x4 transform)
    {
        int currVertex = index;
        int nextVertex = (index + 1) % Vertices.Length;
        return 
            Vector2.Transform(Vertices[nextVertex], transform)
            - 
            Vector2.Transform(Vertices[currVertex], transform);
    }

    public int FindIncidentEdge(Vector2 normal, Matrix4x4 transform)
    {
        int indexIncidentEdge = 0;
        float minProj = float.MaxValue;

        for (int i = 0; i < Vertices.Length; i++)
        {
            var edgeNormal = EdgeAt(i, transform).Normal();
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
        float dist0 = Math.Math.Cross((contactsIn[0] - c0), normal);
        float dist1 = Math.Math.Cross((contactsIn[1] - c0), normal);

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

    private static IEnumerable<Vector2> ParticleToPosition(Particle[] particles)
    {
        if (particles == null || particles.Length == 0) throw new ArgumentException(nameof(particles));

        foreach (Particle particle in particles)
        {
            yield return particle.Position;
        }
    }
}

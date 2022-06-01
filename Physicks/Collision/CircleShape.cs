using System.Numerics;

namespace Physicks.Collision;

public class CircleShape : IShape
{
    public CircleShape(
        float radius,
        Vector2 position,
        float mass)
    {
        Radius = radius;
        Position = position;
        
        Mass = mass;
        InverseMass = 1.0f / mass;
        MomentOfInertia = 0.5f * Radius * Radius;
        InverseMomentOfInertia = 1.0f / MomentOfInertia;
    }

    public Vector2 Position { get; set; }
    public float Radius { get; set; }
    public float Mass { get; set; }
    public float InverseMass { get; set; }
    public float MomentOfInertia { get; set; }
    public float InverseMomentOfInertia { get; set; }
}
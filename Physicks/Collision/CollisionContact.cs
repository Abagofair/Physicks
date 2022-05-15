using System.Numerics;

namespace Physicks.Collision;

public class CollisionContact
{
    public CollisionContact(
        Vector2 startPosition,
        Vector2 endPosition,
        Vector2 normal,
        float depth)
    {
        StartPosition = startPosition;
        EndPosition = endPosition;
        Normal = normal;
        Depth = depth;
    }

   // public Body A { get; }
   // public Body B { get; }
    public Vector2 StartPosition { get; }
    public Vector2 EndPosition { get; }
    public Vector2 Normal { get; }
    public float Depth { get; }

    //ProjectionMethod
    public void ResolvePenetration(Body a, Body b)
    {
        if (!a.IsKinematic)
        {
            float da = Depth / (a.InverseMass + b.InverseMass) * a.InverseMass;
            a.Position -= Normal * da;
        }

        if (!b.IsKinematic)
        {
            float db = Depth / (a.InverseMass + b.InverseMass) * b.InverseMass;
            b.Position += Normal * db;
        }
    }

    //ImpulseMethod
    public void ResolvePenetrationByImpulse(Body a, Body b)
    {
        ResolvePenetration(a, b);

        float e = Math.Min(a.Restitution, b.Restitution);

        float vRelDotNormal = Vector2.Dot(a.Velocity - b.Velocity, Normal);

        //-(1 + elasticityCoefficient)
        float impulseMagnitude = -(1 + e) * vRelDotNormal / (a.InverseMass + b.InverseMass);

        Vector2 impulse = impulseMagnitude * Normal;

        a.ApplyImpulse(impulse);
        b.ApplyImpulse(-impulse);
    }
}
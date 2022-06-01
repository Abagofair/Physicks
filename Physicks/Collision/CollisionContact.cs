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
            a.Position -= Normal * da * 0.8f;
        }

        if (!b.IsKinematic)
        {
            float db = Depth / (a.InverseMass + b.InverseMass) * b.InverseMass;
            b.Position += Normal * db * 0.8f;
        }
    }

    //ImpulseMethod
    public void ResolvePenetrationByImpulse(Body a, Body b)
    {
        ResolvePenetration(a, b);

        float e = System.Math.Min(a.Restitution, b.Restitution);
        float f = System.Math.Min(a.Friction, b.Friction);

        Vector2 ra = EndPosition - a.Position;
        Vector2 rb = StartPosition - b.Position;
        Vector2 va = a.LinearVelocity + new Vector2(-a.AngularVelocity * ra.Y, a.AngularVelocity * ra.X);
        Vector2 vb = b.LinearVelocity + new Vector2(-b.AngularVelocity * rb.Y, b.AngularVelocity * rb.X);
        Vector2 vRel = va - vb;

        float vRelDotNormal = Vector2.Dot(vRel, Normal);

        Vector2 impulseDirection = Normal;

        float det = ((a.InverseMass + b.InverseMass) + Cross(ra, impulseDirection) * Cross(ra, impulseDirection) * a.InverseMomentOfInertia + Cross(rb, impulseDirection) * Cross(rb, impulseDirection) * b.InverseMomentOfInertia);
        float impulseMagnitude = -(1 + e) * vRelDotNormal / det;

        Vector2 impulseAlongNormal = impulseMagnitude * impulseDirection;

        impulseDirection = new Vector2(Normal.Y, -Normal.X);
        vRelDotNormal = Vector2.Dot(vRel, impulseDirection);
        det = ((a.InverseMass + b.InverseMass) + Cross(ra, impulseDirection) * Cross(ra, impulseDirection) * a.InverseMomentOfInertia + Cross(rb, impulseDirection) * Cross(rb, impulseDirection) * b.InverseMomentOfInertia);
        impulseMagnitude = f * -(1 + e) * vRelDotNormal / det;

        Vector2 impulseAlongTangent = impulseMagnitude * impulseDirection + impulseAlongNormal;
        a.ApplyAngularImpulse(impulseAlongTangent, ra);
        b.ApplyAngularImpulse(-impulseAlongTangent, rb);
    }

    private static float Cross(Vector2 a, Vector2 b) => (a.X * b.Y) - (a.Y * b.X);
}
using System.Numerics;

namespace Physicks;

public class CollisionContact
{
    public CollisionContact(
        PhysicsComponent a,
        PhysicsComponent b,
        Vector2 startPosition,
        Vector2 endPosition,
        Vector2 normal,
        float depth)
    {
        A = a ?? throw new ArgumentNullException(nameof(a));
        B = b ?? throw new ArgumentNullException(nameof(b));
        StartPosition = startPosition;
        EndPosition = endPosition;
        Normal = normal;
        Depth = depth;
    }

    public PhysicsComponent A { get; }
    public PhysicsComponent B { get; }
    public Vector2 StartPosition { get; }
    public Vector2 EndPosition { get; }
    public Vector2 Normal { get; }
    public float Depth { get; }

    //ProjectionMethod
    public void ResolvePenetration()
    {
        if (!A.IsKinematic)
        {
            float da = Depth / (A.InverseMass + B.InverseMass) * A.InverseMass;
            A.Position -= Normal * da;
        }

        if (!B.IsKinematic)
        {
            float db = Depth / (A.InverseMass + B.InverseMass) * B.InverseMass;
            B.Position += Normal * db;
        }
    }

    //ImpulseMethod
    public void ResolvePenetrationByImpulse()
    {
        ResolvePenetration();

        float e = Math.Min(A.Restitution, B.Restitution);
        
        float vRelDotNormal = Vector2.Dot(A.Velocity - B.Velocity, Normal);

        //-(1 + elasticityCoefficient)
        float impulseMagnitude = -(1 + e) * vRelDotNormal / (A.InverseMass + B.InverseMass);

        Vector2 impulse = impulseMagnitude * Normal * World.PixelsPerMeter;

        A.ApplyImpulse(impulse);
        B.ApplyImpulse(-impulse);
    }

    public static CollisionContact FromCircleCircleCollision(PhysicsComponent a, PhysicsComponent b)
    {
        if (a == null) throw new ArgumentNullException(nameof(a));
        if (b == null) throw new ArgumentNullException(nameof(b));

        if (a.Shape is CircleShape aShape && b.Shape is CircleShape bShape)
        {
            Vector2 aToB = b.Position - a.Position;
            Vector2 normal = Vector2.Normalize(aToB);
            Vector2 start = b.Position - normal * bShape.Radius;
            Vector2 end = a.Position + normal * aShape.Radius;
            float depth = Vector2.Distance(end, start);

            return new CollisionContact(
                a,
                b,
                start,
                end,
                normal,
                depth);
        }

        throw new ArgumentException("One or both of the arguments was not a circle shape");
    }
}
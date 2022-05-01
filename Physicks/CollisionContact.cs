using System.Numerics;

namespace Physicks;

public class CollisionContact
{
    public CollisionContact(
        PhysicsObject a,
        PhysicsObject b,
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

    public PhysicsObject A { get; }
    public PhysicsObject B { get; }
    public Vector2 StartPosition { get; }
    public Vector2 EndPosition { get; }
    public Vector2 Normal { get; }
    public float Depth { get; }

    //ProjectionMethod
    public void ResolvePenetration()
    {
        if (A.IsKinematic && B.IsKinematic)
            return;

        float da = Depth / (A.InverseMass * B.InverseMass) * A.InverseMass;
        float db = Depth / (A.InverseMass * B.InverseMass) * B.InverseMass;

        A.Position -= Normal * da;
        B.Position += Normal * db;
    }

    public static CollisionContact FromCircleCircle(PhysicsObject a, PhysicsObject b)
    {
        if (a == null) throw new ArgumentNullException(nameof(a));
        if (b == null) throw new ArgumentNullException(nameof(b));

        if (a.Shape is CircleShape aShape && b.Shape is CircleShape bShape)
        {
            Vector2 aToB = b.Position - a.Position;
            Vector2 normal = Vector2.Normalize(aToB);
            Vector2 start = b.Position - normal * bShape.Radius;
            Vector2 end = a.Position + normal * aShape.Radius;
            float depth = Vector2.Distance(start, end);

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
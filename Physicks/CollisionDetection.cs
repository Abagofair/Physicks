using System.Numerics;

namespace Physicks;

public class CollisionDetection
{
    public static bool IsColliding(PhysicsObject a, PhysicsObject b)
    {
        if (a == null) throw new ArgumentNullException(nameof(a));
        if (b == null) throw new ArgumentNullException(nameof(b));

        if (a.Shape is CircleShape && b.Shape is CircleShape)
            return IsCollidingCircleCircle(a, b, out CollisionContact? collisionContact);
        return false;
    }

    public static bool IsCollidingCircleCircle(PhysicsObject a, PhysicsObject b, 
        out CollisionContact? collisionContact)
    {
        if (a == null) throw new ArgumentNullException(nameof(a));
        if (b == null) throw new ArgumentNullException(nameof(b));

        collisionContact = null;

        CircleShape? aAsCircle = a.Shape as CircleShape;
        CircleShape? bAsCircle = b.Shape as CircleShape;

        if (aAsCircle == null) throw new InvalidCastException(nameof(a));
        if (bAsCircle == null) throw new InvalidCastException(nameof(b));

        float radiusSum = aAsCircle.Radius + bAsCircle.Radius;

        bool isColliding = Vector2.DistanceSquared(a.Position, b.Position) <= radiusSum * radiusSum;

        if (isColliding)
        {
            collisionContact = CollisionContact.FromCircleCircle(a, b);
            return true;
        }

        return false;
    }
}
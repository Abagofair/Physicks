using System.Numerics;
using GameUtilities;

namespace Physicks;

public class CollisionDetection
{
    public static bool IsColliding(PhysicsComponent a, PhysicsComponent b, out CollisionContact? collisionContact)
    {
        if (a == null) throw new ArgumentNullException(nameof(a));
        if (b == null) throw new ArgumentNullException(nameof(b));

        collisionContact = null;

        //todo: fix this casting bullshit
        if (a.Shape is CircleShape && b.Shape is CircleShape)
        {
            return IsCollidingCircleCircle(a, b, out collisionContact);
        }
        if (a.Shape is PolygonShape && b.Shape is PolygonShape)
        {
            return IsCollidingPolygonPolygon(a, b, out collisionContact);
        }

        return false;
    }

    public static bool IsCollidingCircleCircle(PhysicsComponent a, PhysicsComponent b, 
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

        bool isColliding = Vector2.DistanceSquared(a.Position, b.Position) <= (radiusSum * radiusSum);

        if (isColliding)
        {
            collisionContact = CollisionContact.FromCircleCircleCollision(a, b);
            return true;
        }

        return false;
    }

    public static float FindMinimumSeparation(PhysicsComponent a, PhysicsComponent b, out Vector2 axis, out Vector2 point)
    {
        if (a == null) throw new ArgumentNullException(nameof(a));
        if (b == null) throw new ArgumentNullException(nameof(b));

        PolygonShape? polygonA = a.Shape as PolygonShape;
        PolygonShape? polygonB = b.Shape as PolygonShape;

        if (polygonA == null) throw new InvalidCastException(nameof(a));
        if (polygonB == null) throw new InvalidCastException(nameof(b));

        float separation = float.MinValue;
        axis = Vector2.Zero;
        point = Vector2.Zero;

        for (int i = 0; i < polygonA.Vertices.Length; i++)
        {
            var va1 = a.WorldPosition(polygonA.Vertices[i]);
            var vb1 = a.WorldPosition(polygonA.Vertices[(i + 1) % polygonA.Vertices.Length]);
            var normal = (vb1 - va1).Normal();

            float minimumSeparation = float.MaxValue;
            Vector2 minimumVertex = Vector2.Zero;
            for (int j = 0; j < polygonB.Vertices.Length; j++)
            {
                Vector2 vertexB = b.WorldPosition(polygonB.Vertices[j]);
                float projection = Vector2.Dot((vertexB - va1), normal);
                if (projection < minimumSeparation)
                {
                    minimumSeparation = projection;
                    minimumVertex = vertexB;
                }
            }
            if (minimumSeparation > separation)
            {
                separation = minimumSeparation;
                axis = vb1 - va1;
                point = minimumVertex;
            }
        }
        return separation;
    }

    public static bool IsCollidingPolygonPolygon(PhysicsComponent a, PhysicsComponent b,
        out CollisionContact? collisionContact)
    {
        if (a == null) throw new ArgumentNullException(nameof(a));
        if (b == null) throw new ArgumentNullException(nameof(b));

        collisionContact = null;

        PolygonShape? polygonA = a.Shape as PolygonShape;
        PolygonShape? polygonB = b.Shape as PolygonShape;

        if (polygonA == null) throw new InvalidCastException(nameof(a));
        if (polygonB == null) throw new InvalidCastException(nameof(b));

        float minSepAB = FindMinimumSeparation(a, b, out Vector2 axisAB, out Vector2 pointAB);
        if (minSepAB >= 0.0f)
        {

            return false;
        }

        float minSepBA = FindMinimumSeparation(b, a, out Vector2 axisBA, out Vector2 pointBA);
        if (minSepBA >= 0.0f)
        {
            return false;
        }

        if (minSepAB > minSepBA)
        {
            Vector2 normal = axisAB.Normal();
            collisionContact = new CollisionContact(
                a,
                b,
                pointAB,
                pointAB + normal * minSepAB,
                normal,
                -minSepAB);
        }
        else
        {
            Vector2 normal = axisBA.Normal();
            collisionContact = new CollisionContact(
                a,
                b,
                pointBA,
                pointBA - normal * minSepBA,
                -normal,
                -minSepBA);
        }

        return true;
    }
}
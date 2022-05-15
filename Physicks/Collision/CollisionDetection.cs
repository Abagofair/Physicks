using System.Numerics;

namespace Physicks.Collision;

public class CollisionDetection
{
    public static bool IsCollidingCircleCircle(ICollideable a, ICollideable b, out CollisionContact? collisionContact)
    {
        if (a == null) throw new ArgumentNullException(nameof(a));
        if (b == null) throw new ArgumentNullException(nameof(b));

        collisionContact = null;

        CircleShape? aAsCircle = a.Shape as CircleShape;
        CircleShape? bAsCircle = b.Shape as CircleShape;

        if (aAsCircle == null || bAsCircle == null) return false;

        float radiusSum = aAsCircle.Radius + bAsCircle.Radius;

        bool isColliding = Vector2.DistanceSquared(a.Position, b.Position) <= (radiusSum * radiusSum);

        if (isColliding)
        {
            Vector2 aToB = b.Position - a.Position;
            Vector2 normal = Vector2.Normalize(aToB);
            Vector2 start = b.Position - normal * bAsCircle.Radius;
            Vector2 end = a.Position + normal * aAsCircle.Radius;
            float depth = Vector2.Distance(end, start);

            collisionContact = new CollisionContact(
                start,
                end,
                normal,
                depth);

            return true;
        }

        return false;
    }

    public static float FindMinimumSeparation(ICollideable a, ICollideable b, out Vector2 axis, out Vector2 point)
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
            var ba = (vb1 - va1);
            var normal = Vector2.Normalize(new Vector2(ba.Y, -ba.X));

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

    public static bool IsCollidingPolygonPolygon(ICollideable a, ICollideable b, out CollisionContact? collisionContact)
    {
        if (a == null) throw new ArgumentNullException(nameof(a));
        if (b == null) throw new ArgumentNullException(nameof(b));

        collisionContact = null;

        PolygonShape? polygonA = a.Shape as PolygonShape;
        PolygonShape? polygonB = b.Shape as PolygonShape;

        if (polygonA == null || polygonB == null) return false;

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
            Vector2 normal = Vector2.Normalize(new Vector2(axisAB.Y, -axisAB.X));
            var depth = -minSepAB;
            var start = pointAB;
            var end = pointAB + normal * depth;

            collisionContact = new CollisionContact(
                start,
                end,
                normal,
                depth);
        }
        else
        {
            Vector2 normal = -Vector2.Normalize(new Vector2(axisBA.Y, -axisBA.X));
            var depth = -minSepBA;
            var start = pointBA - normal * depth;
            var end = pointBA;

            collisionContact = new CollisionContact(
                start,
                end,
                normal,
                depth);
        }

        return true;
    }

    public static bool IsCollidingPolygonCircle(ICollideable polygon, ICollideable circle, out CollisionContact? collisionContact)
    {
        if (polygon == null) throw new ArgumentNullException(nameof(polygon));
        if (circle == null) throw new ArgumentNullException(nameof(circle));

        collisionContact = null;

        PolygonShape? polygonA = polygon.Shape as PolygonShape;
        CircleShape? circleB = circle.Shape as CircleShape;

        if (polygonA == null || circleB == null) return false;

        float minA = float.MinValue;
        float minB = float.MinValue;
        for (int i = 0; i < polygonA.Vertices.Length; i++)
        {
            Vector2 va1 = polygon.WorldPosition(polygonA.Vertices[i]);
            Vector2 vb1 = polygon.WorldPosition(polygonA.Vertices[(i + 1) % polygonA.Vertices.Length]);

            minA = Math.Min(minA, Vector2.DistanceSquared(va1, circle.Position));
            minB = Math.Min(minB, Vector2.DistanceSquared(vb1, circle.Position));
        }

        return false;
    }
}
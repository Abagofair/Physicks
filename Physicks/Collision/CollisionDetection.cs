using System.Numerics;
using Physicks.Math;

namespace Physicks.Collision;

public class CollisionDetection
{
    public static bool IsCollidingCircleCircle(Collideable a, Collideable b, out List<CollisionContact> collisionContacts)
    {
        if (a == null) throw new ArgumentNullException(nameof(a));
        if (b == null) throw new ArgumentNullException(nameof(b));

        collisionContacts = new List<CollisionContact>();

        CircleShape? circleA = a.Shape as CircleShape;
        CircleShape? circleB = b.Shape as CircleShape;

        if (circleA == null || circleB == null) return false;

        float radiusSum = circleA.Radius + circleB.Radius;

        bool isColliding = Vector2.DistanceSquared(circleA.Position, circleB.Position) <= (radiusSum * radiusSum);

        if (isColliding)
        {
            Vector2 aToB = circleB.Position - circleA.Position;
            Vector2 normal = Vector2.Normalize(aToB);
            Vector2 start = circleB.Position - normal * circleB.Radius;
            Vector2 end = circleA.Position + normal * circleA.Radius;
            float depth = Vector2.Distance(end, start);

            var collisionContact = new CollisionContact(
                start,
                end,
                normal,
                depth);

            collisionContacts.Add(collisionContact);

            return true;
        }

        return false;
    }

    public static float FindMinimumSeparation(Collideable a, Collideable b, out int indexReferenceEdge, out Vector2 supportPoint)
    {
        if (a == null) throw new ArgumentNullException(nameof(a));
        if (b == null) throw new ArgumentNullException(nameof(b));

        PolygonShape? polygonA = a.Shape as PolygonShape;
        PolygonShape? polygonB = b.Shape as PolygonShape;

        if (polygonA == null) throw new InvalidCastException(nameof(a));
        if (polygonB == null) throw new InvalidCastException(nameof(b));

        float separation = float.MinValue;
        indexReferenceEdge = 0;
        supportPoint = Vector2.Zero;

        for (int i = 0; i < polygonA.Vertices.Length; i++)
        {
            var va1 = Vector2.Transform(polygonA.Vertices[i], a.Particle.Transform);//a.WorldPosition(polygonA.Vertices[i]);
            var vb1 = Vector2.Transform(polygonA.Vertices[(i + 1) % polygonA.Vertices.Length], a.Particle.Transform);
            var ba = vb1 - va1;
            var normal = ba.Normal();

            float minimumSeparation = float.MaxValue;
            Vector2 minimumVertex = Vector2.Zero;
            for (int j = 0; j < polygonB.Vertices.Length; j++)
            {
                Vector2 vertexB = Vector2.Transform(polygonB.Vertices[j], b.Particle.Transform);
                float projection = Vector2.Dot(vertexB - va1, normal);
                if (projection < minimumSeparation)
                {
                    minimumSeparation = projection;
                    minimumVertex = vertexB;
                }
            }
            if (minimumSeparation > separation)
            {
                separation = minimumSeparation;
                indexReferenceEdge = i;
                supportPoint = minimumVertex;
            }
        }
        return separation;
    }

    public static bool IsCollidingPolygonPolygon(Collideable a, Collideable b, 
        out List<CollisionContact> collisionContacts)
    {
        if (a == null) throw new ArgumentNullException(nameof(a));
        if (b == null) throw new ArgumentNullException(nameof(b));

        collisionContacts = new List<CollisionContact>();

        PolygonShape? aPolygonShape = a.Shape as PolygonShape;
        PolygonShape? bPolygonShape = b.Shape as PolygonShape;

        if (aPolygonShape == null || bPolygonShape == null) return false;

        float abSeparation = FindMinimumSeparation(a, b, out int aIndexReferenceEdge, out _);
        if (abSeparation >= 0.0f)
        {
            return false;
        }

        float baSeparation = FindMinimumSeparation(b, a, out int bIndexReferenceEdge, out _);
        if (baSeparation >= 0.0f)
        {
            return false;
        }

        PolygonShape referenceShape;
        PolygonShape incidentShape;
        int indexReferenceEdge;
        Matrix4x4 referenceTransform;
        Matrix4x4 incidentTransform;

        if (abSeparation > baSeparation)
        {
            referenceShape = aPolygonShape;
            incidentShape = bPolygonShape;
            indexReferenceEdge = aIndexReferenceEdge;
            referenceTransform = a.Particle.Transform;
            incidentTransform = b.Particle.Transform;
        }
        else
        {
            referenceShape = bPolygonShape;
            incidentShape = aPolygonShape;
            indexReferenceEdge = bIndexReferenceEdge;
            referenceTransform = b.Particle.Transform;
            incidentTransform = a.Particle.Transform;
        }

        Vector2 referenceEdge = referenceShape.EdgeAt(indexReferenceEdge, referenceTransform);
        Vector2 referenceEdgeNormal = referenceEdge.Normal();

        int incidentIndex = incidentShape.FindIncidentEdge(referenceEdgeNormal, incidentTransform);
        int incidentNextIndex = (incidentIndex + 1) % incidentShape.Vertices.Length;

        Vector2 v0 = Vector2.Transform(incidentShape.Vertices[incidentIndex], incidentTransform);
        Vector2 v1 = Vector2.Transform(incidentShape.Vertices[incidentNextIndex], incidentTransform);

        var contactPoints = new List<Vector2>()
        {
            v0, v1
        };
        var clippedPoints = new List<Vector2>(contactPoints);

        for (int i = 0; i < referenceShape.Vertices.Length; i++)
        {
            if (i == indexReferenceEdge)
            {
                continue;
            }

            Vector2 c0 = Vector2.Transform(referenceShape.Vertices[i], referenceTransform);
            Vector2 c1 = Vector2.Transform(referenceShape.Vertices[(i + 1) % referenceShape.Vertices.Length], referenceTransform);

            int numClipped = referenceShape.ClipSegmentToLine(contactPoints, clippedPoints, c0, c1);
            if (numClipped < 2)
            {
                break;
            }

            contactPoints.Clear();
            contactPoints.AddRange(clippedPoints);
        }

        Vector2 vref = Vector2.Transform(referenceShape.Vertices[indexReferenceEdge], referenceTransform);

        foreach (var vclip in clippedPoints)
        {
            float separation = Vector2.Dot(vclip - vref, referenceEdgeNormal);

            if (separation <= 0)
            {
                Vector2 contactNormal = referenceEdgeNormal;
                Vector2 contactStart;
                Vector2 contactEnd;

                if (baSeparation >= abSeparation)
                {
                    contactStart = vclip + contactNormal * -separation;
                    contactEnd = vclip;
                    contactNormal *= -1;
                }
                else
                {
                    contactStart = vclip;
                    contactEnd = vclip + contactNormal * -separation;
                }
                
                collisionContacts.Add(new CollisionContact(
                    contactStart,
                    contactEnd,
                    contactNormal,
                    0.0f));
            }
        }

        return true;
    }

    public static bool IsCollidingPolygonCircle(Collideable polygon, Collideable circle, out List<CollisionContact> collisionContacts)
    {
        if (polygon == null) throw new ArgumentNullException(nameof(polygon));
        if (circle == null) throw new ArgumentNullException(nameof(circle));

        collisionContacts = new List<CollisionContact>();

        PolygonShape? polygonA = polygon.Shape as PolygonShape;
        CircleShape? circleShape = circle.Shape as CircleShape;

        if (polygonA == null || circleShape == null) return false;

        float distanceCircleEdge = float.MinValue;
        Vector2 a = Vector2.Zero;
        Vector2 b = Vector2.Zero;
        Vector2 edgeNormal = Vector2.Zero;

        Vector2 circleWorldPosition = Vector2.Transform(circleShape.Position, circle.Particle.Transform);

        bool isOutside = false;

        for (int i = 0; i < polygonA.Vertices.Length; i++)
        {
            Vector2 va1 = Vector2.Transform(polygonA.Vertices[i], polygon.Particle.Transform);//polygon.WorldPosition(polygonA.Vertices[i]);
            Vector2 vb1 = Vector2.Transform(polygonA.Vertices[(i + 1) % polygonA.Vertices.Length], polygon.Particle.Transform);//polygon.WorldPosition(polygonA.Vertices[(i + 1) % polygonA.Vertices.Length]);
            var edge = vb1 - va1;

            var va1ToCircle = circleWorldPosition - va1;
            var normal = Vector2.Normalize(new Vector2(edge.Y, -edge.X));

            float projection = Vector2.Dot(va1ToCircle, normal);

            if (projection > 0.0f)
            {
                distanceCircleEdge = projection;
                edgeNormal = normal;
                a = va1;
                b = vb1;
                isOutside = true;
                break;
            }
            else
            {
                if (projection > distanceCircleEdge)
                {
                    distanceCircleEdge = projection;
                    edgeNormal = normal;
                    a = va1;
                    b = vb1;
                }
            }
        }

        if (isOutside)
        {
            var v2 = b - a;
            var bToA = a - b;
            var v1 = circleWorldPosition - a;
            var bToCircle = circleWorldPosition - b;

            if (Vector2.Dot(v2, v1) < 0.0f)
            {
                if (v1.Length() <= circleShape.Radius)
                {
                    float depth = circleShape.Radius - v1.Length();
                    var normal = Vector2.Normalize(v1);
                    var start = circleWorldPosition + (normal * -circleShape.Radius);

                    var collisionContact = new CollisionContact(
                        startPosition: start,
                        endPosition: start + (normal * depth),
                        normal: normal,
                        depth: depth);

                    collisionContacts.Add(collisionContact);

                    return true;
                }
            }
            else if (Vector2.Dot(bToA, bToCircle) < 0.0f)
            {
                if (bToCircle.Length() <= circleShape.Radius)
                {
                    float depth = circleShape.Radius - bToCircle.Length();
                    var normal = Vector2.Normalize(bToCircle);
                    var start = circleWorldPosition + (normal * -circleShape.Radius);

                    var collisionContact = new CollisionContact(
                        startPosition: start,
                        endPosition: start + (normal * depth),
                        normal: normal,
                        depth: depth);

                    collisionContacts.Add(collisionContact);

                    return true;
                }
            }
            else
            {
                if (distanceCircleEdge <= circleShape.Radius)
                {
                    float depth = circleShape.Radius - distanceCircleEdge;
                    var normal = Vector2.Normalize(edgeNormal);
                    var start = circleWorldPosition - (normal * circleShape.Radius);

                    var collisionContact = new CollisionContact(
                        startPosition: start,
                        endPosition: start + (normal * depth),
                        normal: normal,
                        depth: depth);

                    collisionContacts.Add(collisionContact);

                    return true;
                }
            }

            return false;
        }
        else
        {
            float depth = circleShape.Radius - distanceCircleEdge;
            var normal = edgeNormal;
            var start = circleWorldPosition - (normal * circleShape.Radius);

            var collisionContact = new CollisionContact(
                startPosition: start,
                endPosition: start + (normal * depth),
                normal: normal,
                depth: depth);

            collisionContacts.Add(collisionContact);

            return true;
        }
    }
}
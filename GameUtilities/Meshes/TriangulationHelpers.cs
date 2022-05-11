using System.Numerics;
using GameUtilities.Triangulation;

namespace GameUtilities.Meshes;

public static class MeshHelpers
{
    public static Vertex[] GetVertices(Triangulation triangulation)
    {
        if (triangulation == null) return Array.Empty<Vertex>();

        var vertices = new List<Vertex>();

        foreach (Triangle triangle in triangulation.Triangles)
        {
            vertices.AddRange(triangle.DistinctVertices());
        }

        return vertices.ToArray();
    }

    public static IEnumerable<Vector2> PointsFromCircle(float radius, int points)
    {
        if (radius < 0) throw new ArgumentException(nameof(radius));

        double stepRadians = (Math.PI * 2.0) / points;
        double current = 0.0;

        for (int i = 0; i < points; i++)
        {
            float x = (float)Math.Cos(current);
            float y = (float)Math.Sin(current);

            if (1.0f - Math.Abs(x) <= 0.05)
                x = Math.Sign(x) == 1 ? 1.0f : -1.0f;
            if (1.0f - Math.Abs(y) <= 0.05)
                y = Math.Sign(y) == 1 ? 1.0f : -1.0f;

            var point = new Vector2(x, y);
            point *= radius;

            current += stepRadians;

            yield return point;
        }
    }

    public static Triangulation? Delaunay_BowyerWatson(IEnumerable<Vector2> positions)
    {
        if (positions == null) throw new ArgumentNullException(nameof(positions));

        var va = new Vertex(new Vector2(300.0f, -10000.0f));
        var vb = new Vertex(new Vector2(3000.0f, 5000.0f));
        var vc = new Vertex(new Vector2(-3000.0f, 5000.0f));

        var a = new Edge(va, vb);
        var b = new Edge(vb, vc);
        var c = new Edge(vc, va);

        Triangle superTriangle = new Triangle(a, b, c);

        var edges = new List<Edge>();
        var triangulation = new Triangulation();
        triangulation.Triangles.Add(superTriangle);

        var badTriangles = new List<Triangle>();

        foreach (Vector2 position in positions)
        {
            badTriangles.Clear();

            foreach (Triangle potientalBadTriangle in triangulation.Triangles)
            {
                if (IsPositionInsideCircumCircle(position, potientalBadTriangle))
                {
                    badTriangles.Add(potientalBadTriangle);
                }
            }

            if (badTriangles.Count == 0) continue;

            edges.Clear();

            //bad
            foreach (Triangle badTriangle in badTriangles)
            {
                if (!badTriangles.Any(x => badTriangle != x && x.ContainsEdge(badTriangle.A)))
                {
                    edges.Add(badTriangle.A);
                }

                if (!badTriangles.Any(x => badTriangle != x && x.ContainsEdge(badTriangle.B)))
                {
                    edges.Add(badTriangle.B);
                }

                if (!badTriangles.Any(x => badTriangle != x && x.ContainsEdge(badTriangle.C)))
                {
                    edges.Add(badTriangle.C);
                }
            }

            triangulation.Triangles.RemoveAll(x => badTriangles.Contains(x));

            foreach (Edge edge in edges)
            {
                var pointToEdgeA = new Edge(new Vertex(position), new Vertex(edge.A.Position));
                var pointToEdgeB = new Edge(new Vertex(edge.B.Position), new Vertex(position));
                var tri = new Triangle(pointToEdgeA, edge, pointToEdgeB);
                triangulation.Triangles.Add(tri);
            }
        }

        var goodTriangles = new List<Triangle>();
        foreach (Triangle triangle in triangulation.Triangles)
        {
            if (triangle.ContainsVertex(superTriangle.A.A) || triangle.ContainsVertex(superTriangle.B.A) || triangle.ContainsVertex(superTriangle.C.A))
            {
                continue;
            }

            goodTriangles.Add(triangle);
        }

        triangulation.Triangles = goodTriangles;

        return triangulation;
    }

    public static bool IsPositionInsideCircumCircle(Vector2 position, Triangle triangle)
    {
        if (triangle == null) throw new ArgumentNullException(nameof(triangle));

        Vector2 a, b, c;

        var posA = a = triangle.A.A.Position;
        var posB = b = triangle.B.A.Position;
        var posC = c = triangle.C.A.Position;

        Vector2 center = (posA + posB + posC) / 3;

        double aAngle = Math.Acos(Vector2.Dot(posA, center) / (posA.Length() * center.Length()));
        double bAngle = Math.Acos(Vector2.Dot(posB, center) / (posB.Length() * center.Length()));
        double cAngle = Math.Acos(Vector2.Dot(posC, center) / (posC.Length() * center.Length()));

        if (aAngle >= bAngle && aAngle >= cAngle)
        {
            a = posA;

            if (bAngle >= cAngle)
            {
                b = posB;
                c = posC;
            }
            else
            {
                b = posC;
                c = posB;
            }
        }
        else if (bAngle >= aAngle && bAngle >= cAngle)
        {
            a = posB;

            if (aAngle >= cAngle)
            {
                b = posA;
                c = posC;
            }
            else
            {
                b = posC;
                c = posA;
            }
        }
        else if (cAngle >= aAngle && cAngle >= bAngle)
        {
            a = posC;

            if (aAngle > bAngle)
            {
                b = posA;
                c = posB;
            }
            else
            {
                b = posB;
                c = posA;
            }
        }

        var isccw = IsCCW(a, b, c);

        if (isccw > 0)
        {
            var row1 = CircumCircleMatrixRow(a);
            var row2 = CircumCircleMatrixRow(b);
            var row3 = CircumCircleMatrixRow(c);
            var row4 = CircumCircleMatrixRow(position);
            var matrix = new Matrix4x4(
                row1.X, row1.Y, row1.Z, row1.W,
                row2.X, row2.Y, row2.Z, row2.W,
                row3.X, row3.Y, row3.Z, row3.W,
                row4.X, row4.Y, row4.Z, row4.W);
            return matrix.GetDeterminant() > 0;
        }
        else if (isccw < 0)
        {
            var row1 = CircumCircleMatrixRow(c);
            var row2 = CircumCircleMatrixRow(b);
            var row3 = CircumCircleMatrixRow(a);
            var row4 = CircumCircleMatrixRow(position);
            var matrix = new Matrix4x4(
                row1.X, row1.Y, row1.Z, row1.W,
                row2.X, row2.Y, row2.Z, row2.W,
                row3.X, row3.Y, row3.Z, row3.W,
                row4.X, row4.Y, row4.Z, row4.W);
            return matrix.GetDeterminant() > 0;
        }
        else
        {
            return false;
        }
    }

    private static float IsCCW(Vector2 a, Vector2 b, Vector2 c) => ((b.X - a.X) * (c.Y - a.Y)) - (c.X - a.X) * (b.Y - a.Y);

    private static Vector4 CircumCircleMatrixRow(Vector2 position) =>
        new Vector4(
            position.X,
            position.Y,
            position.X * position.X + position.Y * position.Y,
            1.0f);

    public static bool IsPositionInTriangle(Vector2 position, Vector2 p0, Vector2 p1, Vector2 p2)
    {
        var s = (p0.X - p2.X) * (position.Y - p2.Y) - (p0.Y - p2.Y) * (position.X - p2.X);
        var t = (p1.X - p0.X) * (position.Y - p0.Y) - (p1.Y - p0.Y) * (position.X - p0.X);

        if ((s < 0) != (t < 0) && s != 0 && t != 0)
            return false;

        var d = (p2.X - p1.X) * (position.Y - p1.Y) - (p2.Y - p1.Y) * (position.X - p1.X);
        return d == 0 || (d < 0) == (s + t <= 0);
    }
}
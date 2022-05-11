using System.Numerics;

namespace GameUtilities.Triangulation;

public class Triangle : IEquatable<Triangle>
{
    public Triangle(Edge a, Edge b, Edge c)
    {
        A = a ?? throw new ArgumentNullException(nameof(a));
        B = b ?? throw new ArgumentNullException(nameof(b));
        C = c ?? throw new ArgumentNullException(nameof(c));
    }

    public Edge A { get; private set; }
    public Edge B { get; private set; }
    public Edge C { get; private set; }

    public IEnumerable<Vertex> DistinctVertices()
    {
        var v1 = A.A;
        var v2 = A.B;
        var v3 = B.B;

        return (new Vertex[] { v1, v2, v3 });
    }

    public bool IsComplete() => A != null && B != null && C != null;

    public bool IsPositionInTriangle(Vector2 position)
    {
        var trianglePoints = DistinctVertices().ToArray();
        var p0 = trianglePoints[0].Position;
        var p1 = trianglePoints[1].Position;
        var p2 = trianglePoints[2].Position;

        var s = (p0.X - p2.X) * (position.Y - p2.Y) - (p0.Y - p2.Y) * (position.X - p2.X);
        var t = (p1.X - p0.X) * (position.Y - p0.Y) - (p1.Y - p0.Y) * (position.X - p0.X);

        if ((s < 0) != (t < 0) && s != 0 && t != 0)
            return false;

        var d = (p2.X - p1.X) * (position.Y - p1.Y) - (p2.Y - p1.Y) * (position.X - p1.X);
        return d == 0 || (d < 0) == (s + t <= 0);
    }

    public bool ContainsVertex(Vertex vertex)
    {
        if (vertex == null) return false;

        if (A.A == vertex)
            return true;
        if (B.A == vertex)
            return true;
        if (C.A == vertex)
            return true;
        return false;
    }

    public bool ContainsAnyEdgeFromTriangle(Triangle triangle)
    {
        if (triangle == null) return false;

        if (ContainsEdge(triangle.A))
            return true;
        if (ContainsEdge(triangle.B))
            return true;
        if (ContainsEdge(triangle.C))
            return true;

        return false;
    }

    public bool ContainsEdge(Edge edge)
    {
        if (edge == null) return false;

        if (A == edge)
            return true;
        if (B == edge)
            return true;
        if (C == edge)
            return true;

        return false;
    }

    public bool Equals(Triangle? other)
    {
        if (other == null)
            return false;

        if (ReferenceEquals(this, other))
            return true;

        //first combination
        if (A == other.A && B == other.B && C == other.C)
            return true;

        //second combination
        if (A == other.C && B == other.A && C == other.B)
            return true;

        //third combination
        if (A == other.B && B == other.C && C == other.A)
            return true;

        //fourth combination
        if (A == other.A && B == other.C && C == other.B)
            return true;

        //fifth combination
        if (A == other.B && B == other.A && C == other.C)
            return true;

        //sixth combination
        if (A == other.C && B == other.B && C == other.A)
            return true;

        return false;
    }

    public override string ToString() => $"Edge A: {A.A.Position}, Edge B: {B.A.Position}, Edge C: {C.A.Position}";

    public override int GetHashCode() => HashCode.Combine(A, B, C) ^
                                         HashCode.Combine(C, A, B) ^
                                         HashCode.Combine(B, C, A) ^
                                         HashCode.Combine(B, A, C) ^
                                         HashCode.Combine(C, B, A) ^
                                         HashCode.Combine(A, C, B);

    public override bool Equals(object? obj) => Equals(obj as Triangle);

    public static bool operator ==(Triangle? left, Triangle? right)
    {
        if (left is null) return right is null;

        return left.Equals(right);
    }

    public static bool operator !=(Triangle left, Triangle right)
    {
        if (left is null) return right is null;

        return !left.Equals(right);
    }
}

namespace GameUtilities.Triangulation;

public class Edge : IEquatable<Edge>
{
    public Edge(Vertex a, Vertex b)
    {
        A = a ?? throw new ArgumentNullException(nameof(a));
        B = b ?? throw new ArgumentNullException(nameof(b));
    }

    public Vertex A { get; private set; }
    public Vertex B { get; private set; }

    public bool Equals(Edge? other)
    {
        if (other is null) return false;

        if (ReferenceEquals(this, other)) return true;

        if (A == other.A && B == other.B ||
            A == other.B && B == other.A)
            return true;

        return false;
    }

    public override string ToString() => $"Vertex A: {A}, Vertex B: {B}";

    public override int GetHashCode() => HashCode.Combine(A, B) ^ HashCode.Combine(B, A);

    public override bool Equals(object? obj) => Equals(obj as Edge);

    public static bool operator ==(Edge? left, Edge? right)
    {
        if (left is null) return right is null;

        return left.Equals(right);
    }

    public static bool operator !=(Edge? left, Edge? right)
    {
        if (left is null) return right is null;

        return left.Equals(right);
    }
}
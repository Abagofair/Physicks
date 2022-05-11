using System.Numerics;

namespace GameUtilities.Triangulation;

public class Vertex : IEquatable<Vertex>
{
    public Vertex(Vector2 position)
    {
        Position = position;
    }

    public Vector2 Position { get; set; }

    public bool Equals(Vertex? other)
    {
        if (other == null)
            return false;

        if (ReferenceEquals(this, other))
            return true;

        if (Position == other.Position)
            return true;

        return false;
    }

    public override string ToString() => $"Position: {Position}";

    public override bool Equals(object? obj) => Equals(obj as Edge);

    public override int GetHashCode() => Position.GetHashCode();

    public static bool operator ==(Vertex? left, Vertex? right)
    {
        if (left is null) return right is null;

        return left.Equals(right);
    }

    public static bool operator !=(Vertex? left, Vertex? right)
    {
        if (left is null) return right is null;

        return left.Equals(right);
    }
}

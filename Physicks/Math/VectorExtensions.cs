using System.Numerics;

namespace Physicks.Math;

internal static class VectorExtensions
{
    internal static Vector2 Normal(this Vector2 vector2)
    {
        return Vector2.Normalize(new Vector2(vector2.Y, -vector2.X));
    }
}
using System.Numerics;

namespace Physicks.Math;

public static class Math
{
    public static float Cross(Vector2 a, Vector2 b) => (a.X * b.Y) - (a.Y * b.X);
}
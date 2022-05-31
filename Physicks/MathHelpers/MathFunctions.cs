using System.Numerics;

namespace Physicks.MathHelpers;

public class MathFunctions
{
    public static float Cross(Vector2 a, Vector2 b) => (a.X * b.Y) - (a.Y * b.X);
}
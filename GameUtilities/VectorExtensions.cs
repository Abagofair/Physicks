using System.Numerics;

namespace GameUtilities;

public static class VectorExtensions
{
    public static Vector2 Rotate(this Vector2 vector2, float angle)
    {
        float x = (float)(vector2.X * Math.Cos(angle) - vector2.Y * Math.Sin(angle));
        float y = (float)(vector2.X * Math.Sin(angle) + vector2.Y * Math.Cos(angle));
        
        return new Vector2(x, y);
    }
}
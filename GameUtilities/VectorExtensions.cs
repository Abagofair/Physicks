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

    public static Vector2 Normal(this Vector2 vector2)
    {
        return Vector2.Normalize(new Vector2(vector2.Y, -vector2.X));
    }
}
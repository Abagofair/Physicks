namespace MonoGameUtilities;
public static class VectorExtensions
{
    public static Microsoft.Xna.Framework.Vector2 ToXnaVector2(this System.Numerics.Vector2 vector2) => new(vector2.X, vector2.Y);
}

namespace MonoGameUtilities;
public static class MathExtensions
{
    public static Microsoft.Xna.Framework.Vector2 ToXnaVector2(this System.Numerics.Vector2 vector2) => new(vector2.X, vector2.Y);
    public static Microsoft.Xna.Framework.Matrix ToXnaMatrix4x4(this System.Numerics.Matrix4x4 matrix) =>
        new(
            matrix.M11, matrix.M12, matrix.M13, matrix.M14,
            matrix.M21, matrix.M22, matrix.M23, matrix.M24,
            matrix.M31, matrix.M32, matrix.M33, matrix.M33,
            matrix.M41, matrix.M42, matrix.M43, matrix.M44
            );
}

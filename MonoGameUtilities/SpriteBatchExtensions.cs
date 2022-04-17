using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGameUtilities
{
    public static class SpriteBatchExtensions
    {
		/// <summary>
		/// Draws a line from point1 to point2 with an offset
		/// </summary>
		/// <param name="spriteBatch">The destination drawing surface</param>
		/// <param name="point">The starting point</param>
		/// <param name="length">The length of the line</param>
		/// <param name="angle">The angle of this line from the starting point</param>
		/// <param name="color">The color to use</param>
		/// <param name="thickness">The thickness of the line</param>
		public static void DrawLine(this SpriteBatch spriteBatch, Vector2 point, float length, float angle, Color color, float thickness)
		{
			Texture2D pixel = new Texture2D(spriteBatch.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
			pixel.SetData(new[] { Color.White });

			// stretch the pixel between the two vectors
			spriteBatch.Draw(pixel,
							 point,
							 null,
							 color,
							 angle,
							 new Vector2(0, (float)pixel.Height / 2),
							 new Vector2(length, thickness),
							 SpriteEffects.None,
							 0);
		}

		/// <summary>
		/// Draws a line from point1 to point2 with an offset
		/// </summary>
		/// <param name="spriteBatch">The destination drawing surface</param>
		/// <param name="point1">The first point</param>
		/// <param name="point2">The second point</param>
		/// <param name="color">The color to use</param>
		/// <param name="thickness">The thickness of the line</param>
		public static void DrawLine(this SpriteBatch spriteBatch, Vector2 point1, Vector2 point2, Color color, float thickness)
		{
			// calculate the distance between the two vectors
			float distance = Vector2.Distance(point1, point2);

			// calculate the angle between the two vectors
			float angle = (float)Math.Atan2(point2.Y - point1.Y, point2.X - point1.X);

			DrawLine(spriteBatch, point1, distance, angle, color, thickness);
		}
	}
}

using System.Numerics;

namespace Physicks
{
    public class BoxShape : PolygonShape
    {
        public BoxShape(float width, float height)
        {
            Width = width;
            Height = height;

            Vertices = new Vector2[]
            {
                new Vector2(-Width / 2.0f, -Height / 2.0f),
                new Vector2(Width / 2.0f, -Height / 2.0f),
                new Vector2(Width / 2.0f, Height / 2.0f),
                new Vector2(-Width / 2.0f, Height / 2.0f)
            };
        }

        public float Width { get; }
        public float Height { get; }
        public new float MomentOfInertia => 0.083333f * Width * Width * Height * Height;
    }
}

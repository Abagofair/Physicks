using System.Numerics;

namespace Physicks
{
    public class PolygonShape : IShape
    {
        public PolygonShape()
        {
            Vertices = Array.Empty<Vector2>();
        }

        public Vector2[] Vertices { get; set; }
        public float MomentOfInertia { get; set; }
    }
}

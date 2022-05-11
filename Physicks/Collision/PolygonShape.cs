using System.Numerics;

namespace Physicks.Collision;

public class PolygonShape : IShape
{
    public PolygonShape()
    {
        Vertices = Array.Empty<Vector2>();
    }

    public Vector2[] Vertices { get; set; }
    public virtual float MomentOfInertia { get; set; }
}

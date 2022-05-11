namespace Physicks.Collision;

public class CircleShape : IShape
{
    public CircleShape(float radius)
    {
        Radius = radius;
    }

    public float Radius { get; set; }
    public float MomentOfInertia => 0.5f * Radius * Radius;
}

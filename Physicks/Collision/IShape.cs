namespace Physicks.Collision;

public interface IShape
{
    float Mass { get; set; }
    float InverseMass { get; set; }
    float MomentOfInertia { get; set; }
    float InverseMomentOfInertia { get; set; }
}

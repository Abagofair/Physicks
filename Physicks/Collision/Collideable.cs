using System.Numerics;

namespace Physicks.Collision;

public class Collideable
{
    public Collideable(
        Particle particle,
        IShape shape)
    {
        Particle = particle ?? throw new ArgumentNullException(nameof(particle));
        Shape = shape ?? throw new ArgumentNullException(nameof(shape));
    }

    public Particle Particle { get; }
    public IShape Shape { get; }
}
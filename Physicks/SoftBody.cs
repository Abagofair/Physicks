using System.Numerics;
using Physicks.Collision;

namespace Physicks;

public class SoftBody : Particle
{
    public SoftBody(
        Particle[] particles,
        SoftBodyEdge[] softBodyEdges,
        Vector2 position,
        float mass,
        float momentOfInertia,
        float restitution,
        float friction,
        bool isFixedRotation,
        ParticleType particleType)
        : base(
            position,
            mass,
            momentOfInertia,
            restitution,
            friction,
            isFixedRotation,
            particleType)
    {
        if (particles == null || particles.Length == 0) throw new ArgumentException("Cannot be empty or null", nameof(particles));
        if (softBodyEdges == null || softBodyEdges.Length == 0) throw new ArgumentException("Cannot be empty or null", nameof(softBodyEdges));

        Particles = particles;
        SoftBodyEdges = softBodyEdges;

        //create a shape that matches the structure of the particles
        Shape = new PolygonShape(particles, momentOfInertia);
    }

    public readonly Particle[] Particles;
    public readonly SoftBodyEdge[] SoftBodyEdges;
    public readonly PolygonShape Shape;

    public void Update(ReadOnlySpan<Particle> particles)
    {
        ReadOnlySpan<SoftBodyEdge> springsSpan = SoftBodyEdges.AsSpan();
        ReadOnlySpan<Particle> particleSpan = Particles.AsSpan();

        for (int i = 0; i < springsSpan.Length; i++)
        {
            SoftBodyEdge spring = springsSpan[i];
            Particle left = particleSpan[spring.LeftParticleIndex];
            Particle right = particleSpan[spring.RightParticleIndex];

            left.Force += SpringForce(ref left, ref right, ref spring);
            right.Force += SpringForce(ref right, ref left, ref spring);
        }
    }

    //todo: global static force class
    private static Vector2 SpringForce(
        ref Particle left,
        ref Particle right,
        ref SoftBodyEdge spring)
    {
        Vector2 d = left.Position - right.Position;

        float displacement = d.Length() - spring.RestLength;

        Vector2 springDirection = Vector2.Normalize(d);
        float springMagnitude = -spring.Factor * displacement;

        Vector2 springForce = springDirection * springMagnitude;

        return springForce;
    }
}
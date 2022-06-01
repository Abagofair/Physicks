using System.Numerics;
using Physicks.Math;

namespace Physicks;

public class SoftBody : Particle
{
    public SoftBody(
        Particle[] particles,
        SoftBodyEdge[] softBodyEdges,
        Vector2 position,
        float restitution,
        float friction,
        bool isFixedRotation,
        ParticleType particleType)
        : base(
            position,
            restitution,
            friction,
            isFixedRotation,
            particleType)
    {
        if (particles == null || particles.Length == 0) throw new ArgumentException("Cannot be empty or null", nameof(particles));
        if (softBodyEdges == null || softBodyEdges.Length == 0) throw new ArgumentException("Cannot be empty or null", nameof(softBodyEdges));

        Particles = particles;
        SoftBodyEdges = softBodyEdges;
    }

    public readonly Particle[] Particles;
    public readonly SoftBodyEdge[] SoftBodyEdges;

    public void Update()
    {
        ReadOnlySpan<SoftBodyEdge> springsSpan = SoftBodyEdges.AsSpan();
        ReadOnlySpan<Particle> particleSpan = Particles.AsSpan();

        for (int i = 0; i < springsSpan.Length; i++)
        {
            SoftBodyEdge spring = springsSpan[i];
            Particle left = particleSpan[spring.LeftParticleIndex];
            Particle right = particleSpan[spring.RightParticleIndex];

            left.Force += Forces.Spring(ref left, ref right, ref spring);
            right.Force += Forces.Spring(ref right, ref left, ref spring);
        }
    }
}
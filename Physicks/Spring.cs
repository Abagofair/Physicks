using System.Numerics;

namespace Physicks;

public struct SoftBodyEdge
{
    public SoftBodyEdge(
        byte leftParticleIndex,
        byte rightParticleIndex,
        float factor,
        float restLength)
    {
        LeftParticleIndex = leftParticleIndex;
        RightParticleIndex = rightParticleIndex;
        Factor = factor;
        RestLength = restLength;
    }

    public float Factor;
    public float RestLength;
    public byte LeftParticleIndex;
    public byte RightParticleIndex;
}

//public class MassSpringSystem
//{
//    private ReadOnlyMemory<Spring> _springs;

//    public MassSpringSystem(
//        Spring[] springs)
//    {
//        _springs = new ReadOnlyMemory<Spring>(springs);
//    }

//    public void Update(ReadOnlySpan<Particle> particles)
//    {
//        ReadOnlySpan<Spring> springsSpan = _springs.Span;

//        for (int i = 0; i < springsSpan.Length; i++)
//        {
//            Spring spring = springsSpan[i];
//            Particle left = particles[spring.LeftParticleIndex];
//            Particle right = particles[spring.RightParticleIndex];

//            left.Force += SpringForce(ref left, ref right, ref spring);
//            right.Force += SpringForce(ref right, ref left, ref spring);
//        }
//    }

//    private static Vector2 SpringForce(
//        ref Particle left,
//        ref Particle right,
//        ref Spring spring)
//    {
//        Vector2 d = left.Position - right.Position;

//        float displacement = d.Length() - spring.RestLength;

//        Vector2 springDirection = Vector2.Normalize(d);
//        float springMagnitude = -spring.Factor * displacement;

//        Vector2 springForce = springDirection * springMagnitude;

//        return springForce;
//    }
//}
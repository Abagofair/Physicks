using System.Numerics;

namespace Physicks;

public class SemiImplicitEuler : IIntegrator
{
    public void IntegrateForces(Particle particle, float dt)
    {
        particle.LinearAcceleration = particle.Force * particle.InverseMass;
        particle.LinearVelocity += particle.LinearAcceleration * dt;

        if (!particle.IsFixedRotation)
        {
            particle.AngularAcceleration = particle.Torque * particle.InverseMomentOfInertia;
            particle.AngularVelocity += particle.AngularAcceleration * dt;
        }

        particle.Force = Vector2.Zero;
        particle.Torque = 0.0f;
    }

    public void IntegrateVelocities(Particle particle, float dt)
    {
        particle.Position += particle.LinearVelocity * dt;

        if (!particle.IsFixedRotation)
        {
            particle.Rotation += particle.AngularVelocity * dt;
        }
    }
}
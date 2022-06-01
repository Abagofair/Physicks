using Physicks.Collision;

namespace Physicks;

public interface IIntegrator
{
    void IntegrateForces(Particle particle, IShape shape, float dt);
    void IntegrateVelocities(Particle particle, float dt);
}
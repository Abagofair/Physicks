namespace Physicks;

public interface IIntegrator
{
    void IntegrateForces(Particle particle, float dt);
    void IntegrateVelocities(Particle particle, float dt);
}
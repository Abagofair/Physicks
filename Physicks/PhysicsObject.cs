using System.Numerics;

namespace Physicks;

public class PhysicsObject
{
    public bool IsKinematic { get; set; }
    public Vector2 ForceSum { get; set; }
    public Vector2 Position { get; set; }
    public Vector2 Velocity { get; set; }
    public Vector2 Acceleration { get; set; }

    private float _mass = 1.0f;
    public float Mass
    {
        get => _mass;
        set
        {
            _mass = value;
            InverseMass = 1.0f / _mass;
        }
    }
    public float InverseMass { get; private set; } = 1.0f;

    public ICollideable? Collideable { get; set; }

    //https://gafferongames.com/post/integration_basics/
    public void Integrate(float dt)
    {
        Acceleration = ForceSum * InverseMass;
        Velocity += Acceleration * dt;
        Position += Velocity * dt;
        ForceSum = Vector2.Zero;
    }

    public void AddForce(Vector2 force) => ForceSum += force;
}
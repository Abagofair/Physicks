using System.Numerics;

namespace Physicks;

public class PhysicsObject
{
    public bool IsKinematic { get; set; }
    public Vector2 ForceSum { get; set; }
    public Vector2 Position { get; set; }
    public Vector2 Velocity { get; set; }
    public Vector2 Acceleration { get; set; }
    public float TorqueSum { get; set; }
    public float Rotation { get; set; }
    public float AngularVelocity { get; set; }
    public float AngularAcceleration { get; set; }
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
    public IShape? Shape { get; set; }
    //cache transform
    public Matrix4x4 Transform => Matrix4x4.CreateRotationZ(Rotation) * Matrix4x4.CreateTranslation(new Vector3(Position, 1.0f));
    public float MomentOfInertia => (Shape?.MomentOfInertia ?? 0.0f) * Mass;
    public float InverseMomentOfInertia => 1.0f / MomentOfInertia; //dont calculate every time

    //https://gafferongames.com/post/integration_basics/
    public void Integrate(float dt)
    {
        Acceleration = ForceSum * InverseMass;
        Velocity += Acceleration * dt;
        Position += Velocity * dt;

        if (Shape != null)
        {
            AngularAcceleration = TorqueSum * InverseMomentOfInertia;
            AngularVelocity += AngularAcceleration * dt;
            Rotation += AngularVelocity * dt;
        }

        ForceSum = Vector2.Zero;
        TorqueSum = 0.0f;
    }

    public void AddForce(Vector2 force) => ForceSum += force;
}
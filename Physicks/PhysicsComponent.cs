using System.Numerics;
using System.Text.Json.Serialization;

namespace Physicks;

[Serializable]
public class PhysicsComponent
{
    [JsonInclude]
    public bool IsKinematic { get; set; }

    [JsonInclude]
    public Vector2 Position { get; set; }

    [JsonInclude]
    public float Rotation { get; set; }

    [JsonInclude]
    public float Restitution { get; set; } = 1.0f;

    [JsonInclude]
    public IShape? Shape { get; set; }

    private float _mass = 1.0f;
    [JsonInclude]
    public float Mass
    {
        get => _mass;
        set
        {
            _mass = value;

            if (_mass != 0.0f)
            {
                InverseMass = 1.0f / _mass;
            }
            else
            {
                InverseMass = 0.0f;
            }
        }
    }

    public Vector2 ForceSum { get; set; }

    public Vector2 Velocity { get; set; }

    public Vector2 Acceleration { get; set; }

    public float TorqueSum { get; set; }

    public float AngularVelocity { get; set; }

    public float AngularAcceleration { get; set; }

    public float InverseMass { get; private set; } = 1.0f;

    public ICollideable? Collideable { get; set; }

    //cache transform
    public Matrix4x4 Transform => Matrix4x4.CreateRotationZ(Rotation) * Matrix4x4.CreateTranslation(new Vector3(Position, 0.0f));


    public float MomentOfInertia => (Shape?.MomentOfInertia ?? 0.0f) * Mass;

    public float InverseMomentOfInertia
    {
        get
        {
            if (MomentOfInertia != 0.0f)
                return 1.0f / MomentOfInertia; //dont calculate every time
            return 0.0f;
        }
    }

    public Vector2 WorldPosition(Vector2 offset) => Vector2.Transform(offset, Transform);

    //https://gafferongames.com/post/integration_basics/
    public void Integrate(float dt)
    {
        if (IsKinematic)
            return;

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

    public void AddForce(Vector2 force)
    {
        ForceSum += force;
    }

    public void ApplyImpulse(Vector2 impulse)
    {
        if (IsKinematic)
            return;

        Velocity += impulse * InverseMass;
    }
}
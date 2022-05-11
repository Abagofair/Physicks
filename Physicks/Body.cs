using System.Numerics;
using System.Text.Json.Serialization;
using Physicks.Collision;

namespace Physicks;

[Serializable]
public class Body : IEquatable<Body>, ICollideable
{
    private static int id = 1;

    public Body()
    {
        _id = id++;
    }

    private int _id;
    public int Id => _id;

    [JsonInclude]
    public bool IsKinematic { get; set; }

    [JsonInclude]
    public bool IsFixedRotation { get; set; }

    [JsonInclude]
    public Vector2 Position { get; set; }

    [JsonInclude]
    public float Rotation { get; set; }

    [JsonInclude]
    public float Restitution { get; set; } = 1.0f;

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

    public bool IsTrigger { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public float Scale { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    Matrix4x4 ICollideable.Transform { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public IShape Shape { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public Vector2 WorldPosition(Vector2 offset) => Vector2.Transform(offset, Transform);

    //https://gafferongames.com/post/integration_basics/
    public void Integrate(float dt)
    {
        if (IsKinematic)
            return;

        Acceleration = ForceSum * InverseMass;
        Velocity += Acceleration * dt;
        Position += Velocity * dt;

        if (!IsFixedRotation)
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

    public override int GetHashCode() => Id;

    public bool Equals(Body? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(other, this)) return true;

        return other.Id == Id;
    }

    public override bool Equals(object? obj) => Equals(obj as Body);
}
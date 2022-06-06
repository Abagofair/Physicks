using System.Numerics;
using System.Text.Json.Serialization;
using Physicks.Collision;
using Physicks.Math;

namespace Physicks;

[Serializable]
public class Body : IEquatable<Body>
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

    [JsonInclude]
    public float Friction { get; set; } = 1.0f;

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

    public Vector2 LinearVelocity { get; set; }

    public Vector2 LinearAcceleration { get; set; }

    public float TorqueSum { get; set; }

    public float AngularVelocity { get; set; }

    public float AngularAcceleration { get; set; }

    public float InverseMass { get; private set; } = 1.0f;

    //cache transform
    public Matrix4x4 Transform => Matrix4x4.CreateRotationZ(Rotation) * Matrix4x4.CreateTranslation(new Vector3(Position, 0.0f));
    
    [Obsolete("Should probably only use the transpose since the matrix is orthogonal")]
    public Matrix4x4 InverseTransform
    {
        get
        {
            if (Matrix4x4.Invert(Transform, out Matrix4x4 invertedMatrix))
            {
                return invertedMatrix;
            }
            throw new Exception("Matrix inversion failed");
        }
    }

    public Matrix4x4 PixelsPerMeterTransform => Matrix4x4.CreateRotationZ(Rotation) * Matrix4x4.CreateTranslation((new Vector3(Position, 0.0f) * World.PixelsPerMeter));

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
    //public void Integrate(float dt)
    //{
    //    if (IsKinematic)
    //        return;

    //    LinearAcceleration = ForceSum * InverseMass;
    //    LinearVelocity += LinearAcceleration * dt;
    //    Position += LinearVelocity * dt;

    //    if (!IsFixedRotation)
    //    {
    //        AngularAcceleration = TorqueSum * InverseMomentOfInertia;
    //        AngularVelocity += AngularAcceleration * dt;
    //        Rotation += AngularVelocity * dt;
    //    }

    //    ForceSum = Vector2.Zero;
    //    TorqueSum = 0.0f;
    //}

    public void IntegrateForces(float dt)
    {
        if (IsKinematic)
            return;

        LinearAcceleration = ForceSum * InverseMass;
        LinearVelocity += LinearAcceleration * dt;

        if (!IsFixedRotation)
        {
            AngularAcceleration = TorqueSum * InverseMomentOfInertia;
            AngularVelocity += AngularAcceleration * dt;
        }

        ForceSum = Vector2.Zero;
        TorqueSum = 0.0f;
    }

    public void IntegrateVelocities(float dt)
    {
        if (IsKinematic)
            return;

        Position += LinearVelocity * dt;

        if (!IsFixedRotation)
        {
            Rotation += AngularVelocity * dt;
        }
    }

    public void AddForce(Vector2 force)
    {
        ForceSum += force;
    }

    public void ApplyLinearImpulse(Vector2 impulse)
    {
        if (IsKinematic)
            return;

        LinearVelocity += impulse * InverseMass;
    }

    public void ApplyAngularImpulse(float impulse)
    {
        if (IsKinematic)
            return;

        AngularVelocity += impulse * InverseMomentOfInertia;
    }

    public void ApplyAngularImpulse(Vector2 impulse, Vector2 distanceFromCenterOfMass)
    {
        if (IsKinematic)
            return;

        LinearVelocity += impulse * InverseMass;
        AngularVelocity += Math.Math.Cross(distanceFromCenterOfMass, impulse) * InverseMomentOfInertia;
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
using System.Numerics;
using Physicks.MathHelpers;

namespace Physicks;

public class Particle
{
    private static int id = 1;

    public Particle(
        Vector2 position,
        float mass,
        float momentOfInertia,
        float restitution,
        float friction,
        bool isFixedRotation,
        ParticleType particleType)
    {
        IsFixedRotation = isFixedRotation;

        Type = particleType;

        Mass = mass;
        InverseMass = 1.0f / mass;

        MomentOfInertia = momentOfInertia;
        InverseMomentOfInertia = 1.0f / momentOfInertia;

        Restitution = restitution;
        Friction = friction;

        Position = position;

        _id = id++;
    }

    private readonly int _id;
    public int Id => _id;

    public readonly ParticleType Type;

    public readonly bool IsFixedRotation;

    public readonly float Mass;
    public readonly float InverseMass;

    public readonly float MomentOfInertia;
    public readonly float InverseMomentOfInertia;

    public readonly float Restitution;
    public readonly float Friction;

    public float AngularAcceleration;
    public float AngularVelocity;
    public float Rotation;
    public float Torque;

    public Vector2 LinearAcceleration;
    public Vector2 LinearVelocity;
    public Vector2 Position;
    public Vector2 Force;

    public void AddForce(Vector2 force)
    {
        Force += force;
    }

    public void ApplyLinearImpulse(Vector2 impulse)
    {
        if (IsFixedRotation)
            return;

        LinearVelocity += impulse * InverseMass;
    }

    public void ApplyAngularImpulse(float impulse)
    {
        if (IsFixedRotation)
            return;

        AngularVelocity += impulse * InverseMomentOfInertia;
    }

    public void ApplyAngularImpulse(Vector2 impulse, Vector2 distanceFromCenterOfMass)
    {
        if (IsFixedRotation)
            return;

        LinearVelocity += impulse * InverseMass;
        AngularVelocity += MathFunctions.Cross(distanceFromCenterOfMass, impulse) * InverseMomentOfInertia;
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
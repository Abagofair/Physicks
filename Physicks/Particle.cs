using System.Numerics;

namespace Physicks;

public class Particle
{
    private static int id = 1;

    public Particle(
        Vector2 position,
        float restitution,
        float friction,
        bool isFixedRotation,
        ParticleType particleType)
    {
        IsFixedRotation = isFixedRotation;

        Type = particleType;

        Restitution = restitution;
        Friction = friction;

        Position = position;

        _id = id++;
    }

    private readonly int _id;
    public int Id => _id;

    public readonly ParticleType Type;

    public readonly bool IsFixedRotation;

    public readonly float Restitution;
    public readonly float Friction;

    public readonly float MomentOfInertia;
    public readonly float InverseMomentOfInertia;

    public float AngularAcceleration;
    public float AngularVelocity;
    public float Rotation;
    public float Torque;

    public Vector2 LinearAcceleration;
    public Vector2 LinearVelocity;
    public Vector2 Position;
    public Vector2 Force;

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

    public void AddForce(Vector2 force)
    {
        Force += force;
    }

    public void ApplyLinearImpulse(Vector2 impulse, float invMass)
    {
        if (IsFixedRotation)
            return;

        LinearVelocity += impulse * invMass;
    }

    public void ApplyAngularImpulse(float impulse, float inverseMomentOfInertia)
    {
        if (IsFixedRotation)
            return;

        AngularVelocity += impulse * inverseMomentOfInertia;
    }

    public void ApplyAngularImpulse(Vector2 impulse, Vector2 distanceFromCenterOfMass,
        float inverseMass, float inverseMomentOfInertia)
    {
        if (IsFixedRotation)
            return;

        LinearVelocity += impulse * inverseMass;
        AngularVelocity += Math.Math.Cross(distanceFromCenterOfMass, impulse) * inverseMomentOfInertia;
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
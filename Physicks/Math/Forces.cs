using System.Numerics;

namespace Physicks.Math;

public static class Forces
{
    public static Vector2 Spring(
        ref Particle left,
        ref Particle right,
        ref SoftBodyEdge spring)
    {
        Vector2 d = left.Position - right.Position;

        float displacement = d.Length() - spring.RestLength;

        Vector2 springDirection = Vector2.Normalize(d);
        float springMagnitude = -spring.Factor * displacement;

        Vector2 springForce = springDirection * springMagnitude;

        return springForce;
    }

    public static Vector2 Drag(Body physics2DObject, float dragCoeff)
    {
        if (physics2DObject == null) throw new ArgumentNullException(nameof(physics2DObject));

        return Drag(physics2DObject.LinearVelocity, dragCoeff);
    }

    public static Vector2 Drag(Vector2 velocity, float dragCoeff)
    {
        Vector2 dragForce = Vector2.Zero;

        float magSquared = velocity.LengthSquared();
        if (magSquared > 0)
        {
            dragForce = dragCoeff * magSquared * Vector2.Normalize(velocity) * -1.0f;
        }

        return dragForce;
    }

    public static Vector2 Friction(Body physics2DObject, float frictionCoeff)
    {
        if (physics2DObject == null) throw new ArgumentNullException(nameof(physics2DObject));

        return Friction(physics2DObject.LinearVelocity, frictionCoeff);
    }

    public static Vector2 Friction(Vector2 velocity, float frictionCoeff)
        => frictionCoeff * Vector2.Normalize(velocity) * -1.0f;

    public static Vector2 Gravitational(Body a,
        Body b, float gravitationalCoeff)
    {
        if (a == null) throw new ArgumentNullException(nameof(a));
        if (b == null) throw new ArgumentNullException(nameof(b));

        Vector2 distanceBA = (b.Position - a.Position);
        float distanceBASquared = distanceBA.LengthSquared();

        float attrMag = gravitationalCoeff * (a.Mass * b.Mass) / distanceBASquared;

        return Vector2.Normalize(distanceBA) * attrMag;
    }

    public static Vector2 Spring(Body physics2DObject, Vector2 anchor, float restLength, float k)
    {
        if (physics2DObject == null) throw new ArgumentNullException(nameof(physics2DObject));

        Vector2 d = physics2DObject.Position - anchor;

        float displacement = d.Length() - restLength;

        Vector2 springDirection = Vector2.Normalize(d);
        float springMagnitude = -k * displacement;

        Vector2 springForce = springDirection * springMagnitude;
        return springForce;
    }
}
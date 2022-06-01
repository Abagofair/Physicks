using System.Numerics;
using Physicks.Collision;
using Physicks.Math;

namespace Physicks;

public class PenetrationConstraint : Constraint
{
    public PenetrationConstraint(Collideable first, Collideable second,
        Vector2 firstCollisionPoint, Vector2 secondCollisionPoint, Vector2 normal)
        : base(first, second)
    {
        AnchorPointInFirstBodyLocalSpace = Vector2.Transform(firstCollisionPoint, first.Particle.InverseTransform);
        AnchorPointInSecondBodyLocalSpace = Vector2.Transform(secondCollisionPoint, second.Particle.InverseTransform);
        PenetrationNormalInFirstLocalSpace = Vector2.Transform(normal, first.Particle.InverseTransform);

        Jacobian = new MatMN(2, 6);
        Jacobian.Zero();

        CachedLambda = new VecN(Jacobian.M);
        CachedLambda.Zero();
        Friction = 0.0f;
    }

    public Vector2 AnchorPointInFirstBodyLocalSpace { get; }
    public Vector2 AnchorPointInSecondBodyLocalSpace { get; }
    public Vector2 PenetrationNormalInFirstLocalSpace { get; }
    public float Friction { get; private set; }

    public MatMN Jacobian { get; }
    public VecN CachedLambda { get; private set; }
    public float Bias { get; private set; }

    public override void PreSolve(float dt)
    {
        Vector2 pa = Vector2.Transform(AnchorPointInFirstBodyLocalSpace, First.Particle.Transform);
        Vector2 pb = Vector2.Transform(AnchorPointInSecondBodyLocalSpace, Second.Particle.Transform);
        Vector2 normalInWorldSpace = Vector2.Transform(PenetrationNormalInFirstLocalSpace, First.Particle.Transform);

        Vector2 ra = pa - First.Particle.Position;
        Vector2 rb = pb - Second.Particle.Position;

        Vector2 va = First.Particle.LinearVelocity + new Vector2(-First.Particle.AngularVelocity * ra.Y, First.Particle.AngularVelocity * ra.X);
        Vector2 vb = Second.Particle.LinearVelocity + new Vector2(-Second.Particle.AngularVelocity * rb.Y, Second.Particle.AngularVelocity * rb.X);
        float vRelDotNormal = Vector2.Dot((va - vb), normalInWorldSpace);

        //populate the distance jacobian
        Vector2 J1 = -normalInWorldSpace;//(pa - pb) * 2.0f;
        Jacobian.Rows[0][0] = J1.X; //A linear velocity.X
        Jacobian.Rows[0][1] = J1.Y; //A linear velocity.Y

        float J2 = -Math.Math.Cross(ra, normalInWorldSpace);//Body.Cross(ra, pa - pb) * 2.0f;
        Jacobian.Rows[0][2] = J2; //A angular velocity

        Vector2 J3 = normalInWorldSpace;//(pb - pa) * 2.0f;
        Jacobian.Rows[0][3] = J3.X; //B linear velocity.X
        Jacobian.Rows[0][4] = J3.Y; //B linear velocity.Y

        float J4 = Math.Math.Cross(rb, normalInWorldSpace);//Body.Cross(rb, pb - pa) * 2.0f;
        Jacobian.Rows[0][5] = J4; //A angular velocity

        Friction = System.Math.Max(First.Particle.Friction, Second.Particle.Friction);
        if (Friction > 0.0f)
        {
            var tangent = new Vector2(normalInWorldSpace.Y, -normalInWorldSpace.X);
            Jacobian.Rows[1][0] = -tangent.X;
            Jacobian.Rows[1][1] = -tangent.Y;
            Jacobian.Rows[1][2] = -Math.Math.Cross(ra, tangent);

            Jacobian.Rows[1][3] = tangent.X;
            Jacobian.Rows[1][4] = tangent.Y;
            Jacobian.Rows[1][5] = Math.Math.Cross(rb, tangent);
        }

        //Warm starting
        VecN impulses = Jacobian.Transpose() * CachedLambda;

        First.Particle.ApplyLinearImpulse(new Vector2(impulses[0], impulses[1]), First.Shape.InverseMass);
        First.Particle.ApplyAngularImpulse(impulses[2], First.Shape.InverseMomentOfInertia);

        Second.Particle.ApplyLinearImpulse(new Vector2(impulses[3], impulses[4]), Second.Shape.InverseMass);
        Second.Particle.ApplyAngularImpulse(impulses[5], Second.Shape.InverseMomentOfInertia);

        //baumgartner stabilization factor - smoothness factor
        float e = System.Math.Min(First.Particle.Restitution, Second.Particle.Restitution);

        float beta = 0.2f;
        float positionalError = Vector2.Dot((pb - pa), -normalInWorldSpace);
        positionalError = System.Math.Min(0.0f, positionalError + 0.01f);
        Bias = (beta / dt) * positionalError + (e * vRelDotNormal);
    }

    public override void Solve()
    {
        //compute lambda
        VecN V = GetVelocities();
        MatMN invM = GetInverseMass();

        VecN numerator = Jacobian * V * -1.0f;
        numerator[0] -= Bias;
        MatMN denominator = Jacobian * invM * Jacobian.Transpose();

        VecN lambda = MatMN.SolveGaussSeidel(denominator, numerator);

        // Accumulate umpulses and clamp it within constraint limits
        //note: this gives very small impulses that does not end up solving correctly
        //why is that?
        /*VecN oldLambda = CachedLambda;
        CachedLambda += lambda;
        CachedLambda[0] = CachedLambda[0] < 0.0f
            ? 0.0f
            : CachedLambda[0];

        lambda = CachedLambda - oldLambda;*/
                
        CachedLambda += lambda;

        if (Friction > 0.0f)
        {
            float maxFriction = CachedLambda[0] * Friction;
            float minusMaxFriction = -maxFriction;
            if (minusMaxFriction < maxFriction)
            {
                CachedLambda[1] = System.Math.Clamp(CachedLambda[1], minusMaxFriction, maxFriction);
            }
            else
            {
                CachedLambda[1] = System.Math.Clamp(CachedLambda[1], maxFriction, minusMaxFriction);
            }
        }

        VecN impulses = Jacobian.Transpose() * lambda;

        //apply lambda impulse to first and second body
        First.Particle.ApplyLinearImpulse(new Vector2(impulses[0], impulses[1]), First.Shape.InverseMass);
        First.Particle.ApplyAngularImpulse(impulses[2], First.Shape.InverseMomentOfInertia);

        Second.Particle.ApplyLinearImpulse(new Vector2(impulses[3], impulses[4]), Second.Shape.InverseMass);
        Second.Particle.ApplyAngularImpulse(impulses[5], Second.Shape.InverseMomentOfInertia);
    }

    public override void PostSolve()
    {
    }
}
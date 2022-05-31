using System.Numerics;
using Physicks.MathHelpers;

namespace Physicks;

public class PenetrationConstraint : Constraint
{
    public PenetrationConstraint(Body first, Body second,
        Vector2 firstCollisionPoint, Vector2 secondCollisionPoint, Vector2 normal)
        : base(first, second)
    {
        AnchorPointInFirstBodyLocalSpace = Vector2.Transform(firstCollisionPoint, first.InverseTransform);
        AnchorPointInSecondBodyLocalSpace = Vector2.Transform(secondCollisionPoint, second.InverseTransform);
        PenetrationNormalInFirstLocalSpace = Vector2.Transform(normal, first.InverseTransform);

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
        Vector2 pa = Vector2.Transform(AnchorPointInFirstBodyLocalSpace, First.Transform);
        Vector2 pb = Vector2.Transform(AnchorPointInSecondBodyLocalSpace, Second.Transform);
        Vector2 normalInWorldSpace = Vector2.Transform(PenetrationNormalInFirstLocalSpace, First.Transform);

        Vector2 ra = pa - First.Position;
        Vector2 rb = pb - Second.Position;

        Vector2 va = First.LinearVelocity + new Vector2(-First.AngularVelocity * ra.Y, First.AngularVelocity * ra.X);
        Vector2 vb = Second.LinearVelocity + new Vector2(-Second.AngularVelocity * rb.Y, Second.AngularVelocity * rb.X);
        float vRelDotNormal = Vector2.Dot((va - vb), normalInWorldSpace);

        //populate the distance jacobian
        Vector2 J1 = -normalInWorldSpace;//(pa - pb) * 2.0f;
        Jacobian.Rows[0][0] = J1.X; //A linear velocity.X
        Jacobian.Rows[0][1] = J1.Y; //A linear velocity.Y

        float J2 = -MathFunctions.Cross(ra, normalInWorldSpace);//Body.Cross(ra, pa - pb) * 2.0f;
        Jacobian.Rows[0][2] = J2; //A angular velocity

        Vector2 J3 = normalInWorldSpace;//(pb - pa) * 2.0f;
        Jacobian.Rows[0][3] = J3.X; //B linear velocity.X
        Jacobian.Rows[0][4] = J3.Y; //B linear velocity.Y

        float J4 = MathFunctions.Cross(rb, normalInWorldSpace);//Body.Cross(rb, pb - pa) * 2.0f;
        Jacobian.Rows[0][5] = J4; //A angular velocity

        Friction = Math.Max(First.Friction, Second.Friction);
        if (Friction > 0.0f)
        {
            var tangent = new Vector2(normalInWorldSpace.Y, -normalInWorldSpace.X);
            Jacobian.Rows[1][0] = -tangent.X;
            Jacobian.Rows[1][1] = -tangent.Y;
            Jacobian.Rows[1][2] = -MathFunctions.Cross(ra, tangent);

            Jacobian.Rows[1][3] = tangent.X;
            Jacobian.Rows[1][4] = tangent.Y;
            Jacobian.Rows[1][5] = MathFunctions.Cross(rb, tangent);
        }

        //Warm starting
        VecN impulses = Jacobian.Transpose() * CachedLambda;

        First.ApplyLinearImpulse(new Vector2(impulses[0], impulses[1]));
        First.ApplyAngularImpulse(impulses[2]);

        Second.ApplyLinearImpulse(new Vector2(impulses[3], impulses[4]));
        Second.ApplyAngularImpulse(impulses[5]);

        //baumgartner stabilization factor - smoothness factor
        float e = Math.Min(First.Restitution, Second.Restitution);

        float beta = 0.2f;
        float positionalError = Vector2.Dot((pb - pa), -normalInWorldSpace);
        positionalError = Math.Min(0.0f, positionalError + 0.01f);
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
                CachedLambda[1] = Math.Clamp(CachedLambda[1], minusMaxFriction, maxFriction);
            }
            else
            {
                CachedLambda[1] = Math.Clamp(CachedLambda[1], maxFriction, minusMaxFriction);
            }
        }

        VecN impulses = Jacobian.Transpose() * lambda;

        //apply lambda impulse to first and second body
        First.ApplyLinearImpulse(new Vector2(impulses[0], impulses[1]));
        First.ApplyAngularImpulse(impulses[2]);

        Second.ApplyLinearImpulse(new Vector2(impulses[3], impulses[4]));
        Second.ApplyAngularImpulse(impulses[5]);
    }

    public override void PostSolve()
    {
    }
}
using System.Numerics;
using Physicks.Collision;
using Physicks.Math;

namespace Physicks;

public class JointConstraint : Constraint
{
    public JointConstraint(
        Collideable first,
        Collideable second, 
        Vector2 worldSpaceAnchorPoint) 
        : base(
            first, 
            second)
    {
        AnchorPoint = worldSpaceAnchorPoint;
        AnchorPointInFirstBodyLocalSpace = Vector2.Transform(worldSpaceAnchorPoint, first.Particle.InverseTransform);
        AnchorPointInSecondBodyLocalSpace = Vector2.Transform(worldSpaceAnchorPoint, second.Particle.InverseTransform);

        Jacobian = new MatMN(1, 6);
        Jacobian.Zero();

        CachedLambda = new VecN(Jacobian.M);
        CachedLambda.Zero();
    }

    public Vector2 AnchorPoint { get; }
    public Vector2 AnchorPointInFirstBodyLocalSpace { get; }
    public Vector2 AnchorPointInSecondBodyLocalSpace { get; }

    public MatMN Jacobian { get; }
    public VecN CachedLambda { get; private set; }
    public float Bias { get; private set; }

    public override void PreSolve(float dt)
    {
        Vector2 pa = Vector2.Transform(AnchorPointInFirstBodyLocalSpace, First.Particle.Transform);
        Vector2 pb = Vector2.Transform(AnchorPointInSecondBodyLocalSpace, Second.Particle.Transform);

        Vector2 ra = pa - First.Particle.Position;
        Vector2 rb = pb - Second.Particle.Position;

        //populate the distance jacobian
        Vector2 J1 = (pa - pb) * 2.0f;
        Jacobian.Rows[0][0] = J1.X; //A linear velocity.X
        Jacobian.Rows[0][1] = J1.Y; //A linear velocity.Y

        float J2 = Math.Math.Cross(ra, pa - pb) * 2.0f;
        Jacobian.Rows[0][2] = J2; //A angular velocity

        Vector2 J3 = (pb - pa) * 2.0f;
        Jacobian.Rows[0][3] = J3.X; //B linear velocity.X
        Jacobian.Rows[0][4] = J3.Y; //B linear velocity.Y

        float J4 = Math.Math.Cross(rb, pb - pa) * 2.0f;
        Jacobian.Rows[0][5] = J4; //A angular velocity

        //Warm starting
        VecN impulses = Jacobian.Transpose() * CachedLambda;

        First.Particle.ApplyLinearImpulse(new Vector2(impulses[0], impulses[1]), First.Shape.InverseMass);
        First.Particle.ApplyAngularImpulse(impulses[2], First.Shape.InverseMomentOfInertia);

        Second.Particle.ApplyLinearImpulse(new Vector2(impulses[3], impulses[4]), Second.Shape.InverseMass);
        Second.Particle.ApplyAngularImpulse(impulses[5], Second.Shape.InverseMomentOfInertia);

        //baumgartner stabilization factor - smoothness factor
        float beta = 0.1f;
        float positionalError = Vector2.Dot((pb - pa), (pb - pa));
        positionalError = System.Math.Max(0.0f, positionalError - 0.01f);
        Bias = (beta / dt) * positionalError;
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
        CachedLambda += lambda;

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
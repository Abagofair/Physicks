using System.Numerics;
using Physicks.MathHelpers;

namespace Physicks;

public class JointConstraint : Constraint
{
    public JointConstraint(Body first, Body second, Vector2 worldSpaceAnchorPoint) 
        : base(first, second)
    {
        AnchorPointInFirstBodyLocalSpace = Vector2.Transform(worldSpaceAnchorPoint, first.InverseTransform);
        AnchorPointInFirstBodyWorldSpace = Vector2.Transform(AnchorPointInFirstBodyLocalSpace, first.Transform);

        AnchorPointInSecondBodyLocalSpace = Vector2.Transform(worldSpaceAnchorPoint, second.InverseTransform);
        AnchorPointInSecondBodyWorldSpace = Vector2.Transform(AnchorPointInSecondBodyLocalSpace, second.Transform);

        Jacobian = new MatMN(1, 6);
        Jacobian.Zero();
    }

    public Vector2 AnchorPointInFirstBodyLocalSpace { get; }
    public Vector2 AnchorPointInSecondBodyLocalSpace { get; }
    public Vector2 AnchorPointInFirstBodyWorldSpace { get; }
    public Vector2 AnchorPointInSecondBodyWorldSpace { get; }

    public MatMN Jacobian { get; }

    private void PopulateJacobian()
    {
        Vector2 pa = Vector2.Transform(AnchorPointInFirstBodyLocalSpace, First.Transform);
        Vector2 pb = Vector2.Transform(AnchorPointInSecondBodyLocalSpace, Second.Transform);

        Vector2 ra = pa - First.Position;
        Vector2 rb = pb - Second.Position;

        //populate the distance jacobian
        Vector2 J1 = (pa - pb) * 2.0f;
        Jacobian.Rows[0][0] = J1.X; //A linear velocity.X
        Jacobian.Rows[0][1] = J1.Y; //A linear velocity.Y

        float J2 = Body.Cross(ra, pa - pb) * 2.0f;
        Jacobian.Rows[0][2] = J2; //A angular velocity

        Vector2 J3 = (pb - pa) * 2.0f;
        Jacobian.Rows[0][3] = J3.X; //B linear velocity.X
        Jacobian.Rows[0][4] = J3.Y; //B linear velocity.Y

        float J4 = Body.Cross(rb, pb - pa) * 2.0f;
        Jacobian.Rows[0][5] = J4; //A angular velocity
    }

    public override void Solve()
    {
        PopulateJacobian();

        //compute lambda
        VecN V = GetVelocities();
        MatMN invM = GetInverseMass();

        VecN numerator = Jacobian * V * -1.0f;
        MatMN denominator = Jacobian * invM * Jacobian.Transpose();

        VecN lambda = MatMN.SolveGaussSeidel(denominator, numerator);

        VecN impulses = Jacobian.Transpose() * lambda;

        //apply lambda impulse to first and second body
        First.ApplyLinearImpulse(new Vector2(impulses[0], impulses[1]));
        First.ApplyAngularImpulse(impulses[2]);

        Second.ApplyLinearImpulse(new Vector2(impulses[3], impulses[4]));
        Second.ApplyAngularImpulse(impulses[5]);
    }
}
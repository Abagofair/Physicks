using Physicks.MathHelpers;

namespace Physicks;

//TODO: HINGE CONSTRAINT
public abstract class Constraint
{
    public Constraint(
        Body first,
        Body second)
    {
        First = first ?? throw new ArgumentNullException(nameof(first));
        Second = second ?? throw new ArgumentNullException(nameof(second));
    }

    public Body First { get; }
    public Body Second { get; }

    public MatMN GetInverseMass()
    {
        MatMN invM = new(6, 6);
        invM.Zero();

        invM.Rows[0][0] = First.InverseMass;
        invM.Rows[1][1] = First.InverseMass;
        invM.Rows[2][2] = First.InverseMomentOfInertia;

        invM.Rows[3][3] = Second.InverseMass;
        invM.Rows[4][4] = Second.InverseMass;
        invM.Rows[5][5] = Second.InverseMomentOfInertia;

        return invM;
    }

    public VecN GetVelocities()
    {
        VecN vecN = new(6);

        vecN.Zero();

        vecN[0] = First.LinearVelocity.X;
        vecN[1] = First.LinearVelocity.Y;
        vecN[2] = First.AngularVelocity;

        vecN[3] = Second.LinearVelocity.X;
        vecN[4] = Second.LinearVelocity.Y;
        vecN[5] = Second.AngularVelocity;

        return vecN;
    }

    public abstract void PreSolve(float dt);
    public abstract void Solve();
    public abstract void PostSolve();
}
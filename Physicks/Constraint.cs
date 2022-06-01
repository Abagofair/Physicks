using Physicks.Collision;
using Physicks.Math;

namespace Physicks;

//TODO: HINGE CONSTRAINT
public abstract class Constraint
{
    public Constraint(
        Collideable first,
        Collideable second)
    {
        First = first ?? throw new ArgumentNullException(nameof(first));
        Second = second ?? throw new ArgumentNullException(nameof(second));
    }

    public Collideable First { get; }
    public Collideable Second { get; }

    public MatMN GetInverseMass()
    {
        MatMN invM = new(6, 6);
        invM.Zero();

        invM.Rows[0][0] = First.Shape.InverseMass;
        invM.Rows[1][1] = First.Shape.InverseMass;
        invM.Rows[2][2] = First.Shape.InverseMomentOfInertia;

        invM.Rows[3][3] = Second.Shape.InverseMass;
        invM.Rows[4][4] = Second.Shape.InverseMass;
        invM.Rows[5][5] = Second.Shape.InverseMomentOfInertia;

        return invM;
    }

    public VecN GetVelocities()
    {
        VecN vecN = new(6);

        vecN.Zero();

        vecN[0] = First.Particle.LinearVelocity.X;
        vecN[1] = First.Particle.LinearVelocity.Y;
        vecN[2] = First.Particle.AngularVelocity;

        vecN[3] = Second.Particle.LinearVelocity.X;
        vecN[4] = Second.Particle.LinearVelocity.Y;
        vecN[5] = Second.Particle.AngularVelocity;

        return vecN;
    }

    public abstract void PreSolve(float dt);
    public abstract void Solve();
    public abstract void PostSolve();
}
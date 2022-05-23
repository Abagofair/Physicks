using Physicks.MathHelpers;

namespace Physicks;

public class PenetrationConstraint : Constraint
{
    public PenetrationConstraint(Body first, Body second) 
        : base(first, second)
    {
    }

    public override void Solve()
    {
        throw new NotImplementedException();
    }
}
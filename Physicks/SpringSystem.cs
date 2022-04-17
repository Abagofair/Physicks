using System.Numerics;

namespace Physicks;

public class SpringSystem
{
    public SpringSystem()
    {
        Connectors = new List<SpringConnector>();
    }

    public List<SpringConnector> Connectors { get; private set; }

    public void AddConnector(
        PhysicsObject start,
        PhysicsObject end,
        float length,
        float springFactor)
    {
        Connectors.Add(new SpringConnector(
            start,
            end,
            length,
            springFactor));
    }

    public void UpdateForces(World world)
    {
        foreach (SpringConnector connector in Connectors)
        {
            if (!connector.EndAnchor.IsKinematic)
            {
                Vector2 springForce = World.CreateSpringForce(
                    connector.StartAnchor,
                    connector.EndAnchor.Position,
                    //world.TransformToWorldUnit(connector.Length),
                    connector.Length,
                    connector.SpringFactor);

                connector.EndAnchor.AddForce(springForce);
            }
        }
    }
}
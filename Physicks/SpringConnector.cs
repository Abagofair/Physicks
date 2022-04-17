namespace Physicks;

public class SpringConnector
{
    public SpringConnector(
        PhysicsObject startAnchor,
        PhysicsObject endAnchor,
        float length,
        float springFactor)
    {
        StartAnchor = startAnchor ?? throw new ArgumentNullException(nameof(startAnchor));
        EndAnchor = endAnchor ?? throw new ArgumentNullException(nameof(endAnchor));
        SpringFactor = springFactor;
        Length = length;
    }

    public PhysicsObject StartAnchor { get; set; }
    public PhysicsObject EndAnchor { get; set; }
    public float SpringFactor { get; set; }
    public float Length { get; set; }
}
namespace Physicks;

public class SpringConnector
{
    public SpringConnector(
        PhysicsComponent startAnchor,
        PhysicsComponent endAnchor,
        float length,
        float springFactor)
    {
        StartAnchor = startAnchor ?? throw new ArgumentNullException(nameof(startAnchor));
        EndAnchor = endAnchor ?? throw new ArgumentNullException(nameof(endAnchor));
        SpringFactor = springFactor;
        Length = length;
    }

    public PhysicsComponent StartAnchor { get; set; }
    public PhysicsComponent EndAnchor { get; set; }
    public float SpringFactor { get; set; }
    public float Length { get; set; }
}
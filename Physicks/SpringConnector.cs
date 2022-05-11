namespace Physicks;

public class SpringConnector
{
    public SpringConnector(
        Body startAnchor,
        Body endAnchor,
        float length,
        float springFactor)
    {
        StartAnchor = startAnchor ?? throw new ArgumentNullException(nameof(startAnchor));
        EndAnchor = endAnchor ?? throw new ArgumentNullException(nameof(endAnchor));
        SpringFactor = springFactor;
        Length = length;
    }

    public Body StartAnchor { get; set; }
    public Body EndAnchor { get; set; }
    public float SpringFactor { get; set; }
    public float Length { get; set; }
}
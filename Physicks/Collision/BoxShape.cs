using System.Numerics;
using System.Text.Json.Serialization;

namespace Physicks.Collision;

[Serializable]
public class BoxShape : PolygonShape
{
    public BoxShape(float width, float height)
    {
        Width = width;
        Height = height;

        Vertices = new Vector2[]
        {
            new Vector2(-Width / 2.0f, -Height / 2.0f),
            new Vector2(Width / 2.0f, -Height / 2.0f),
            new Vector2(Width / 2.0f, Height / 2.0f),
            new Vector2(-Width / 2.0f, Height / 2.0f)
        };
    }

    [JsonInclude]
    public float Width { get; }

    [JsonInclude]
    public float Height { get; }

    public override float MomentOfInertia => 0.083333f * Width * Width * Height * Height;
}

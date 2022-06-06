using System.Numerics;
using System.Text.Json.Serialization;

namespace Physicks.Collision;

[Serializable]
public class BoxShape : PolygonShape
{
    public BoxShape(
        float width, 
        float height,
        float mass)
        : base(
            new Vector2[]
            {
                new Vector2(-width / 2.0f, -height / 2.0f),
                new Vector2(width / 2.0f, -height / 2.0f),
                new Vector2(width / 2.0f, height / 2.0f),
                new Vector2(-width / 2.0f, height / 2.0f)
            }, 
            0.083333f * (width * width + height * height),
            mass)
    {
        Width = width;
        Height = height;
    }

    [JsonInclude]
    public float Width { get; }

    [JsonInclude]
    public float Height { get; }
}

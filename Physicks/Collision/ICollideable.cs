using System.Numerics;

namespace Physicks.Collision;

public interface ICollideable
{
    int Id { get; }
    bool IsTrigger { get; set; }
    Vector2 Position { get; set; }
    float Rotation { get; set; }
    float Scale { get; set; }
    Matrix4x4 Transform { get; set; }
    Vector2 WorldPosition(Vector2 offset);
    IShape Shape { get; set; }
}
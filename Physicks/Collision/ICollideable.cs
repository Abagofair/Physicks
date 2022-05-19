﻿using System.Numerics;

namespace Physicks.Collision;

public interface ICollideable
{
    int Id { get; }
    Vector2 Position { get; set; }
    float Rotation { get; set; }
    [Obsolete("TODO: Rename WorldPosition")]
    Vector2 WorldPosition(Vector2 offset);
    IShape? Shape { get; set; }
}
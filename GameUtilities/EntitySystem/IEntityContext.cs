﻿namespace GameUtilities.EntitySystem;

public interface IEntityContext
{
    TComponent? Query<TComponent>() where TComponent : class;
    void AddOrOverride<TComponent>(TComponent component) where TComponent : class;
}
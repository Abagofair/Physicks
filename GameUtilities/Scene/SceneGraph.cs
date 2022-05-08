﻿using GameUtilities.EntitySystem;

namespace GameUtilities.Scene;

public class SceneGraph
{
    public Entities<EntityContext> Entities { get; }

    public SceneGraph()
    {
        Entities = new Entities<EntityContext>(100);
    }

    public void AddEntity(EntityContext entityContext)
    {
        if (entityContext == null) throw new ArgumentNullException(nameof(entityContext));

        Entities.CreateEntity(entityContext);
    }
}
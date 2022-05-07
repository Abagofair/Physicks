using GameUtilities.EntitySystem;

namespace GameUtilities.Scene;

public class SceneGraph
{
    private Entities<EntityContext> _entityContexts;

    public SceneGraph()
    {
        _entityContexts = new Entities<EntityContext>(100);
    }

    public void CreateEntities(EntityContext[] entityContexts)
    {
        if (entityContexts == null) throw new ArgumentNullException(nameof(entityContexts));

        foreach (EntityContext entityContext in entityContexts)
        {
            _entityContexts.CreateEntity(entityContext);
        }
    }
}
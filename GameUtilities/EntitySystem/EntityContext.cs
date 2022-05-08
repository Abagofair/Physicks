namespace GameUtilities.EntitySystem;

public class EntityContext : IEntityContext
{
    private readonly Dictionary<Type, object> _componentByType;

    public EntityContext()
    {
        _componentByType = new Dictionary<Type, object>();
    }

    public void AddOrOverride<TComponent>(TComponent component)
        where TComponent : class
    {
        _componentByType[typeof(TComponent)] = component;
    }

    public void AddOrOverride(Type componentType, object component)
    {
        _componentByType[componentType] = component;
    }

    public TComponent? Query<TComponent>()
        where TComponent : class
    {
        if (_componentByType.TryGetValue(typeof(TComponent), out var component))
            return component as TComponent;
        return default;
    }

    public void Clear() => _componentByType.Clear();
}
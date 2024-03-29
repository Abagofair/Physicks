﻿using System.Buffers;

namespace GameUtilities.EntitySystem;

//v0.1
//todo: something better allocated
//e.g. linear component list .. [this range has ComponentA, this range has ComponentB, this range has ComponentC]
public class Entities<TEntityContext> where TEntityContext : IEntityContext
{
    private static int _nextAvailableIndex = 0;
    private static int _currentSize = 0;

    private readonly TEntityContext[] _entityContexts;
    private readonly List<Entity> _entities;

    public Entities(int initialSize)
    {
        _entities = new List<Entity>();
        _entityContexts = new TEntityContext[initialSize];
        _currentSize = initialSize;
    }

    public int CurrentSize => _currentSize;
    public int NextAvailableIndex => _nextAvailableIndex;
    private IEnumerable<TEntityContext> AssignedEntities => _entityContexts.Where(x => x != null);

    public Entity CreateEntity(TEntityContext entityContext)
    {
        if (_nextAvailableIndex >= _currentSize)
        {
            TEntityContext[] resized = new TEntityContext[_currentSize + 50];
            Array.Copy(_entityContexts, resized, _entityContexts.Length);
            _currentSize += 50;
        }

        _entityContexts[_nextAvailableIndex] = entityContext;

        var entity = new Entity(_nextAvailableIndex);

        _entities.Add(entity);

        _nextAvailableIndex += 1;

        return entity;
    }

    public TEntityContext GetEntityContext(ref Entity entity)
    {
        if (entity.Id < 0 || entity.Id > _currentSize) throw new IndexOutOfRangeException(nameof(entity.Id));

        return _entityContexts[entity.Id];
    }

    public IEnumerable<TComponent> Query<TComponent>()
        where TComponent : class
    {
        foreach (var entityContext in AssignedEntities)
        {
            TComponent? component = entityContext.Query<TComponent>();

            if (component != null) yield return component;
        }
    }

    public IEnumerable<(TComponentA, TComponentB)> Query<TComponentA, TComponentB>()
        where TComponentA : class
        where TComponentB : class
    {
        foreach (var entityContext in AssignedEntities)
        {
            TComponentA? componentA = entityContext.Query<TComponentA>();
            TComponentB? componentB = entityContext.Query<TComponentB>();

            if (componentA != null && componentB != null)
                yield return (componentA, componentB);
        }
    }

    public IEnumerable<(TComponentA, TComponentB, TComponentC)> Query<TComponentA, TComponentB, TComponentC>()
        where TComponentA : class
        where TComponentB : class
        where TComponentC : class
    {
        foreach (var entityContext in AssignedEntities)
        {
            TComponentA? componentA = entityContext.Query<TComponentA>();
            TComponentB? componentB = entityContext.Query<TComponentB>();
            TComponentC? componentC = entityContext.Query<TComponentC>();

            if (componentA != null && componentB != null && componentC != null)
                yield return (componentA, componentB, componentC);
        }
    }
}
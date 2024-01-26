using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Roguelike.Entity;

namespace Roguelike;

public class EntityManager : DrawableGameComponent
{
    private List<List<Entity.Entity>> _entities = new(){new List<Entity.Entity>()};

    public List<Entity.Entity> EntitiesOnLevel(int level)
    {
        return _entities.Count >= level ? _entities[level - 1] : new List<Entity.Entity>();
    }
    
    public EntityManager(RoguelikeGame game) : base(game)
    {
        
    }

    public void AddEntityToWorld(Entity.Entity entity)
    {
        while (_entities.Count < entity.Location.Z)
        {
            _entities.Add(new List<Entity.Entity>());
        }
        _entities[entity.Location.Z - 1].Add(entity);
        entity.EntityWasDestroyed += RemoveEntityFromWorld;
    }

    public void RemoveEntityFromWorld(object sender, EventArgs args)
    {
        DestroyEventArgs a = (DestroyEventArgs)args;
        _entities[a.Entity.Location.Z - 1].Remove(a.Entity);
    }
}
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Roguelike.Entity;

namespace Roguelike;

public class EntityManager : DrawableGameComponent
{
    public List<Entity.Entity> Entities = new();
    
    public EntityManager(RoguelikeGame game) : base(game)
    {
        
    }

    public void AddEntityToWorld(Entity.Entity entity)
    {
        Entities.Add(entity);
        entity.EntityWasDestroyed += RemoveEntityFromWorld;
    }

    public void RemoveEntityFromWorld(object sender, EventArgs args)
    {
        DestroyEventArgs a = (DestroyEventArgs)args;
        Entities.Remove(a.Entity);
    }
}
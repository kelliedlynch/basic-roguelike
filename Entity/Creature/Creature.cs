using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Roguelike.Map;

namespace Roguelike.Entity.Creature;

public class Creature : Entity
{
    public bool Ready = false;
    
    public Pathfinder Pathfinder = new ();
    public event EventHandler CreatureWasDestroyed;
    public Creature()
    {
        SpriteLocation = new IntVector2(25, 2);
        Color = Color.Silver;
        EntityName = "Generic Creature";
    }

    public override void Destroy()
    {
        var coin = new Money(1);
        var args = new DestroyEventArgs(this, new List<Entity>() { coin });
        CreatureWasDestroyed?.Invoke(this, args);
    }
    
    public virtual void AttackEntity(Entity entity)
    {
        var damage = Atk - entity.Def;
        entity.TakeDamage(damage);
        var args = new ActivityLogEventArgs($"{Name} hit {entity.Name} for {damage} damage");
        LogEvent(args);
        // entity.Destroy();
    }

    public bool CanSeeEntity(TileMap map, Entity entity)
    {
        Pathfinder.CreaturesBlockPath = false;
        var path = Pathfinder.FindPath(map, Location.To2D, entity.Location.To2D);
        Pathfinder.CreaturesBlockPath = true;
        return (path is not null && path.Count < 10);
    }

}
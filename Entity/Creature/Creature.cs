using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Roguelike.Map;

namespace Roguelike.Entity.Creature;

public class Creature : Entity
{
    public bool Ready = false;
    
    public readonly Pathfinder Pathfinder = new ();

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
        InvokeEntityWasDestroyed(args);
        LogEvent($"{Name} was destroyed");
        // EntityWasDestroyed?.Invoke(this, args);
    }
    
    public virtual void AttackEntity(Entity entity)
    {
        var damage = Atk - entity.Def;
        entity.TakeDamage(damage);
        var args = new ActivityLogEventArgs($"{Name} hit {entity.Name} for {damage} damage");
        LogEvent(args);
    }

    public bool CanSeeEntity(DungeonLevel level, Entity entity)
    {
        // TODO: this probably belongs in a manager
        Pathfinder.CreaturesBlockPath = false;
        var path = Pathfinder.FindPath(level, Location.To2D, entity.Location.To2D);
        Pathfinder.CreaturesBlockPath = true;
        return (path is not null && path.Count < 10);
    }

}
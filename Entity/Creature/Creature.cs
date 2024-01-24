using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Roguelike.Entity.Creature;

public class Creature : Entity
{

    
    public Pathfinder Pathfinder = new ();
    public event EventHandler CreatureWasDestroyed;
    public Creature()
    {
        SpriteLocation = new IntVector2(25, 2);
        Color = Color.Silver;
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
        // entity.Destroy();
    }



}
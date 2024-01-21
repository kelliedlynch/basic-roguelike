using System;
using System.Buffers.Text;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Roguelike.Entity;

namespace Roguelike.Entity;

public class Creature : Roguelike.Entity.Entity
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
}
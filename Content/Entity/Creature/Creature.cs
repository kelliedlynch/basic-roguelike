using System;
using System.Buffers.Text;
using Microsoft.Xna.Framework;

namespace Roguelike.Content.Entity.Creature;

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
        CreatureWasDestroyed?.Invoke(this, new DestroyEventArgs(this));
    }
}
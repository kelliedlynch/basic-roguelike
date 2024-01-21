using System;
using System.Collections.Generic;

namespace Roguelike.Entity;

public class Entity : SpriteRepresented
{
    public IntVector2 Location;

    public event EventHandler EntityWasCreated;
    public event EventHandler EntityWasDestroyed;

    public virtual void Destroy()
    {
        EntityWasDestroyed?.Invoke(this, new DestroyEventArgs(this));
    }


}

public class DestroyEventArgs : EventArgs
{
    public Entity Entity;
    public List<Entity> ItemsDropped = new();

    public DestroyEventArgs(Entity e)
    {
        Entity = e;
    }
    
    public DestroyEventArgs(Entity e, List<Entity> items)
    {
        Entity = e;
        ItemsDropped.AddRange(items);
    }
}
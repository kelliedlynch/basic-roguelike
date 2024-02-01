using System;
using System.Collections.Generic;

namespace Roguelike.Event;

public class DestroyEventArgs : EventArgs
{
    public Entity.Entity Entity;
    public IntVector3 DestroyLocation;
    public List<Entity.Entity> ItemsDropped = new();

    public DestroyEventArgs(Entity.Entity e)
    {
        Entity = e;
        DestroyLocation = e.Location;
    }
    
    public DestroyEventArgs(Entity.Entity e, List<Entity.Entity> items)
    {
        Entity = e;
        DestroyLocation = e.Location;
        ItemsDropped.AddRange(items);
    }
}

public delegate void DestroyEventHandler (Entity.Entity sender, DestroyEventArgs args);
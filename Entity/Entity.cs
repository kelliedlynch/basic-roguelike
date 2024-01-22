using System;
using System.Collections.Generic;

namespace Roguelike.Entity;

public class Entity : SpriteRepresented
{
    public IntVector2 Location
    {
        get => _location;
        set
        {
            if (value == _location) return;
            var args = new MoveEventArgs(this, new IntVector2(Location.X, Location.Y), value);
            _location = value;
            EntityMoved?.Invoke(this, args);
        }
    }

    private IntVector2 _location;
    public int DungeonLevel;

    public event EventHandler EntityWasCreated;
    public event EventHandler EntityWasDestroyed;
    public event EventHandler EntityMoved;

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

public class MoveEventArgs : EventArgs
{
    public Entity Entity;
    public IntVector2 FromLocation;
    public IntVector2 ToLocation;

    public MoveEventArgs(Entity e)
    {
        Entity = e;
    }
    
    public MoveEventArgs(Entity e, IntVector2 from, IntVector2 to)
    {
        Entity = e;
        FromLocation = from;
        ToLocation = to;
    }
}
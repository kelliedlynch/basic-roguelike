using System;
using System.Collections.Generic;

namespace Roguelike.Entity;

public class Entity : SpriteRepresented
{
    // public IntVector2 MapLocation
    // {
    //     get => new (_location.X, _location.Y);
    //     set
    //     {
    //         if (value == MapLocation) return;
    //         var args = new MoveEventArgs(this, new IntVector2(MapLocation.X, MapLocation.Y), value);
    //         _location = new IntVector3(value.X, value.Y, _location.Z);
    //         EntityMoved?.Invoke(this, args);
    //     }
    // }

    public IntVector3 Location
    {
        get => _location;
        set
        {
            if (value == Location) return;
            var args = new MoveEventArgs(this, Location, value);
            _location = value;
            EntityMoved?.Invoke(this, args);
        }
    }

    private IntVector3 _location;

    // public int DungeonLevel
    // {
    //     get => _location.Z;
    // }

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
    public IntVector3 FromLocation;
    public IntVector3 ToLocation;

    public MoveEventArgs(Entity e)
    {
        Entity = e;
    }
    
    public MoveEventArgs(Entity e, IntVector2 from, IntVector2 to)
    {
        var from3d = new IntVector3(from.X, from.Y, e.Location.Z);
        var to3d = new IntVector3(to.X, to.Y, e.Location.Z);
        SetVars(e, from3d, to3d);
    }
    
    public MoveEventArgs(Entity e, IntVector3 from, IntVector3 to)
    {
        SetVars(e, from, to);
    }

    private void SetVars(Entity e, IntVector3 from, IntVector3 to)
    {
        Entity = e;
        FromLocation = from;
        ToLocation = to;
    }
}
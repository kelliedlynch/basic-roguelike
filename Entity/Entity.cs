using System;
using System.Collections.Generic;

namespace Roguelike.Entity;

public class Entity : SpriteRepresented
{
    public int Hp = 10;
    public int Atk = 5;
    public int Def = 1;

    public string EntityName = "Generic Entity";

    public string Name
    {
        get
        {
            if (_name != "") return _name;
            return EntityName;
        }
        set
        {
            _name = value;
        }
    }
    public string _name = "";

    public event EventHandler OnLogEvent;

    public virtual IntVector3 Location
    {
        get => _location;
        set
        {
            if (value == Location) return;
            var oldLoc = Location;
            _location = value;
            // InvokeEntityMoved(oldLoc, _location);
        }
    }

    protected IntVector3 _location;

    // public int DungeonLevel
    // {
    //     get => _location.Z;
    // }

    // public event EventHandler EntityWasCreated;
    public event EventHandler EntityWasDestroyed;
    // public event EventHandler EntityMoved;

    // protected virtual void InvokeEntityMoved(IntVector3 from, IntVector3 to)
    // {
    //     var args = new MoveEventArgs(this, from, to);
    //     EntityMoved?.Invoke(this, args);
    // }

    public virtual void TakeDamage(int dmg)
    {
        var health = Hp - dmg;
        if (health <= 0)
        {
            Hp = 0;
            Destroy();
            return;
        }

        Hp = health;
    }

    public void LogEvent(EventArgs args)
    {
        var a = (ActivityLogEventArgs)args;
        OnLogEvent?.Invoke(this, a);
    }
    
    public void LogEvent(string message)
    {
        var args = new ActivityLogEventArgs(message);
        LogEvent(args);
    }
    
    public virtual void Destroy()
    {
        EntityWasDestroyed?.Invoke(this, new DestroyEventArgs(this));
        LogEvent($"{Name} was destroyed");
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
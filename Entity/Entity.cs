using System;
using System.Collections.Generic;
using Roguelike.Event;

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

    public event DestroyEventHandler EntityWasDestroyed;

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
        InvokeEntityWasDestroyed(new DestroyEventArgs(this));
        LogEvent($"{Name} was destroyed");
    }

    protected void InvokeEntityWasDestroyed(DestroyEventArgs args)
    {
        EntityWasDestroyed?.Invoke(this, args);
    }
}
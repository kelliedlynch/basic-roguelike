using System;

namespace Roguelike.Event;

public class MoveEventArgs : EventArgs
{
    public Entity.Entity Entity;
    public IntVector3 FromLocation;
    public IntVector3 ToLocation;

    public MoveEventArgs(Entity.Entity e)
    {
        Entity = e;
    }
    
    public MoveEventArgs(Entity.Entity e, IntVector2 from, IntVector2 to)
    {
        var from3d = new IntVector3(from.X, from.Y, e.Location.Z);
        var to3d = new IntVector3(to.X, to.Y, e.Location.Z);
        SetVars(e, from3d, to3d);
    }
    
    public MoveEventArgs(Entity.Entity e, IntVector3 from, IntVector3 to)
    {
        SetVars(e, from, to);
    }

    private void SetVars(Entity.Entity e, IntVector3 from, IntVector3 to)
    {
        Entity = e;
        FromLocation = from;
        ToLocation = to;
    }
}

public delegate void MoveEventHandler (MoveEventArgs args);
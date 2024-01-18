using System.Runtime.Intrinsics.X86;
using Microsoft.Xna.Framework.Graphics;

namespace Roguelike;

public class DungeonTile
{
    public int X;
    public int Y;

    public IntVector2 Location => new (X, Y);

    public TileType Type = TileType.Void;
    
    // Fields used only during pathfinding
    public DungeonTile Parent = null;
    public int DistanceToTarget = -1;
    public float Cost;



    public float F
    {
        get
        {
            if (DistanceToTarget != -1 && Cost > -1)
                return DistanceToTarget + Cost;
            else
                return -1;
        }
    }

    public DungeonTile(int x, int y)
    {
        X = x;
        Y = y;
    }


}

public enum TileType : int
{
    Wall = 12,
    Floor = 2,
    Hallway = 3,
    Void = 14
}
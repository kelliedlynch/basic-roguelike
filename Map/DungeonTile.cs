using Microsoft.Xna.Framework;

namespace Roguelike.Map;

public class DungeonTile : SpriteRepresented
{
    public int X;
    public int Y;
    public int Z;

    public IntVector3 Location => new (X, Y, Z);

    private TileType _type = TileType.Void;
    public TileType Type
    {
        get => _type;
        set
        {
            switch (value)
            {
                case TileType.Floor: 
                case TileType.Hallway:
                    SpriteLocation = new IntVector2(2, 0);
                    Color = Color.DarkSlateGray;
                    break;
                case TileType.Wall:
                    SpriteLocation = new IntVector2(10, 17);
                    Color = Color.White;
                    break;
                default:
                    SpriteLocation = new IntVector2(0, 0);
                    Color = Color.Black;
                    break;
            }

            _type = value;
        }
    }
    
    
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
        Color = Color.White;
    }


}

public enum TileType : int
{
    Wall = 12,
    Floor = 2,
    Hallway = 3,
    Void = 14
}
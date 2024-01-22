using Microsoft.Xna.Framework;

namespace Roguelike.Entity.Feature;

public class Portal : Feature
{
    public Portal LinkedPortal;
    public int DungeonLevel;
}

public class StairsUp : Portal
{
    public StairsUp(int x, int y)
    {
        SpriteLocation = new IntVector2(2, 6);
        Location = new IntVector2(x, y);
        Color = Color.Chartreuse;
    }
}

public class StairsDown : Portal
{
    public StairsDown(int x, int y)
    {
        SpriteLocation = new IntVector2(3, 6);
        Location = new IntVector2(x, y);
        Color = Color.Chartreuse;
    }
}
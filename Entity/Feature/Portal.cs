using Microsoft.Xna.Framework;

namespace Roguelike.Entity.Feature;

public class Portal : Feature
{
    public Portal LinkedPortal;
    public int DungeonLevel;
}

public class StairsUp : Portal
{
    public StairsUp(int x, int y, int z)
    {
        SetVars(new IntVector3(x, y, z));
    }
    
    public StairsUp(IntVector3 loc)
    {
        SetVars(loc);
    }
    
    private void SetVars(IntVector3 loc)
    {
        SpriteLocation = new IntVector2(2, 6);
        Location = loc;
        Color = Color.Chartreuse;
    }
}

public class StairsDown : Portal
{
    public StairsDown(int x, int y, int z)
    {
        SetVars(new IntVector3(x, y, z));
    }
    
    public StairsDown(IntVector3 loc)
    {
        SetVars(loc);
    }
    
    private void SetVars(IntVector3 loc)
    {
        SpriteLocation = new IntVector2(3, 6);
        Location = loc;
        Color = Color.Chartreuse;
    }
}
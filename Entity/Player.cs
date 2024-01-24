using Microsoft.Xna.Framework;
using Roguelike.Entity;
using Roguelike.Map;
using StateMachine;

namespace Roguelike.Entity;

public class Player : Creature
{
    public int Money = 0;
    
    public override IntVector3 Location
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

    
    public Player()
    {
        SpriteLocation = new IntVector2(27, 0);
        Color = Color.Aqua;

    }

    public void PickUp(Entity entity)
    {
        if (entity is Money money)
        {
            Money += money.Value;
            entity.Destroy();
        }
    }

    public bool CanMoveToTile(TileMap map, DungeonTile tile)
    {
        var path = Pathfinder.FindPath(map, Location.To2D, tile.Location.To2D);
        if (path is not null && path.Count > 0)
        {
            return true;
        }

        return false;
    }
}
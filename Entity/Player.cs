using Microsoft.Xna.Framework;
using Roguelike.Entity.Creature;
using Roguelike.Entity.Item;
using Roguelike.Map;
using Roguelike.Utility;

namespace Roguelike.Entity;

public class Player : Creature.Creature
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
        Hp = 20;
        Atk = 8;
        Def = 2;
        Name = "Player";
    }

    public void PickUp(Entity entity)
    {
        if (entity is Money money)
        {
            Money += money.Value;
            money.Destroy();
            LogEvent($"{Name} picked up {money.Value} gold");
        }

        if (entity is Weapon weapon)
        {
            Inventory.EquipItem(weapon);
            weapon.Destroy();
            LogEvent($"{Name} equipped {weapon.Name} (Atk {weapon.Atk})");
        }
    }

    public bool CanMoveToTile(DungeonLevel level, DungeonTile tile)
    {
        var path = Pathfinder.FindPath(level, Location.To2D, tile.Location.To2D);
        if (path is not null && path.Count > 0)
        {
            return true;
        }

        return false;
    }
}
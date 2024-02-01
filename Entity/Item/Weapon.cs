using Microsoft.Xna.Framework;

namespace Roguelike.Entity.Item;

public class Weapon : Item
{
    public Weapon(int attack)
    {
        Atk = attack;
        Name = "Generic Weapon";
        SpriteLocation = new IntVector2(32, 8);
        Color = Color.YellowGreen;
    }
}
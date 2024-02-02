using Microsoft.Xna.Framework;
using Roguelike.Utility;

namespace Roguelike.Entity.Item;

public class Weapon : Item
{
    public Weapon(int attack)
    {
        Atk = attack;
        Name = $"Generic Weapon (Atk {Atk})";
        SpriteLocation = new IntVector2(32, 8);
        Color = Color.YellowGreen;
    }
}
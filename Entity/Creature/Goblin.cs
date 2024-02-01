using Microsoft.Xna.Framework;

namespace Roguelike.Entity.Creature;

public class Goblin : Creature
{
    public Goblin()
    {
        Hp = 6;
        Atk = 3;
        Def = 0;
        EntityName = "Goblin";
        Color = Color.Olive;
    }
}
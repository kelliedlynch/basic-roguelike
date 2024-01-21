using Microsoft.Xna.Framework;
using Roguelike.Entity;

namespace Roguelike.Entity;

public class Player : Creature
{
    public int Money = 0;



    
    public Player()
    {
        SpriteLocation = new IntVector2(27, 0);
        Color = Color.Aqua;
    }

    public void AttackEntity(Entity entity)
    {
        entity.Destroy();
    }

    public void PickUp(Entity entity)
    {
        if (entity is Money money)
        {
            Money += money.Value;
            entity.Destroy();
        }
    }
}
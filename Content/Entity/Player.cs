using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Roguelike.Content.Entity;
using Roguelike.Content.Entity.Creature;

namespace Roguelike;

public class Player : Creature
{




    
    public Player()
    {
        SpriteLocation = new IntVector2(27, 0);
        Color = Color.Aqua;
    }

    public void AttackEntity(Entity entity)
    {
        entity.Destroy();
    }
}
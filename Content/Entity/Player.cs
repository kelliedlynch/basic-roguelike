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
    }

    // public void Draw(SpriteBatch spriteBatch)
    // {
    //     var destinationRect =
    //         new Rectangle(Location.X * TileSize.X, Location.Y * TileSize.Y, TileSize.X, TileSize.Y);
    //     var spriteRect = new Rectangle(SpriteLocation.X * TileSize.X, SpriteLocation.Y * TileSize.Y, TileSize.X,
    //         TileSize.Y);
    //     spriteBatch.Draw(SpriteSheet, destinationRect, spriteRect, Color.Aqua);
    // }
}
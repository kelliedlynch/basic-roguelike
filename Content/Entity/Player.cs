using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Roguelike.Content.Entity;

namespace Roguelike;

public class Player : Entity
{



    public Pathfinder Pathfinder = new ();
    
    public Player()
    {
        // var content = new ContentManager(provider, "Content");
        // SpriteSheet = content.Load<Texture2D>("Graphics/monochrome-transparent_packed");
        SpriteSheet = ("Graphics/monochrome-transparent_packed");
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
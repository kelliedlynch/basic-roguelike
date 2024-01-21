using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Roguelike.Content.Entity;

// Class for things that can't be picked up, like stairs or decorative tiles

public class Feature : Roguelike.Entity.Entity
{
    
}

public class StairsUp : Feature
{
    public StairsUp(int x, int y)
    {
        // var content = new ContentManager(provider, "Content");
        // SpriteSheet = content.Load<Texture2D>("Graphics/monochrome-transparent_packed");
        SpriteSheet = ("Graphics/monochrome-transparent_packed");
        SpriteLocation = new IntVector2(2, 6);
        Location = new IntVector2(x, y);
        Color = Color.Chartreuse;
    }
}

public class StairsDown : Feature
{
    public StairsDown(int x, int y)
    {
        // var content = new ContentManager(provider, "Content");
        // SpriteSheet = content.Load<Texture2D>("Graphics/monochrome-transparent_packed");
        SpriteSheet = ("Graphics/monochrome-transparent_packed");
        SpriteLocation = new IntVector2(3, 6);
        Location = new IntVector2(x, y);
        Color = Color.Chartreuse;
    }
}

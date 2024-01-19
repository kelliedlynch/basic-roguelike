using Microsoft.Xna.Framework.Graphics;

namespace Roguelike.Content.Entity;

public class Entity
{
    public IntVector2 Location;
    // protected Texture2D SpriteSheet;
    public string SpriteSheet = "Graphics/monochrome-transparent_packed";
    public IntVector2 SpriteLocation;
    protected IntVector2 TileSize = new(16, 16);
    
}

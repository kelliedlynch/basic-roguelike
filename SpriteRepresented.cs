using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Roguelike.Utility;

namespace Roguelike;

public class SpriteRepresented
{
    public string SpriteSheet = "Graphics/monochrome-transparent_packed";
    public IntVector2 SpriteLocation = new(0, 0);
    public IntVector2 TileSize = new(16, 16);

    public Rectangle Rectangle =>
        new Rectangle(SpriteLocation.X * TileSize.X, SpriteLocation.Y * TileSize.Y, TileSize.X, TileSize.Y);

    public Color Color = Color.Beige;

}
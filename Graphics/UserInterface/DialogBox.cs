using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Roguelike.Graphics;

namespace Roguelike.UserInterface;

public class DialogBox : DrawnElement
{
    
    private const string SpriteSheet = "Graphics/UI_tiles";
    // SpriteLocation is the grid location of the upper left tile of a 3 x 3 block of dialog box tiles
    private static readonly IntVector2 SpriteLocation = new(3, 12);
    protected static readonly IntVector2 TileSize = new(16, 16);

    public Alignment BoxAlignment = Alignment.Center;
    public Alignment TextAlignment = Alignment.Center;

    public string Text = "";
    
    public SpriteFont Font;
    
    public DialogBox(Game game) : base(game)
    {
        Sizing = AxisSizing.FixedXFixedY;
    }

    public DialogBox(Game game, IntVector2 size) : base(game)
    {
        Sizing = AxisSizing.FixedXFixedY;
    }

    protected override void LoadContent()
    {
        Font = Game.Content.Load<SpriteFont>("Fonts/Kenney Mini");
        var stringSize = Font.MeasureString(Text);
        var tilesSize = new IntVector2((int)Math.Ceiling(stringSize.X / TileSize.X),
            (int)Math.Ceiling(stringSize.Y / TileSize.Y));
        Size = (tilesSize + IntVector2.Two) * TileSize;
        base.LoadContent();
    }

    // private IntVector2 FindPosition(Rectangle container, IntVector2 elementSize, Alignment align)
    // {
    //     // var screen = Game.GraphicsDevice.Viewport.Bounds;
    //     var vPadding = container.Height - elementSize.Y;
    //     var hPadding = container.Width - elementSize.X;
    //     var originX = container.X;
    //     var originY = container.Y;
    //     if ((align & Alignment.Center) != 0)
    //     {
    //         originX = container.X + hPadding / 2;
    //         originY = container.Y + vPadding / 2;
    //     }
    //     if ((align & Alignment.Left) != 0)
    //     {
    //         originX = container.X;
    //     } else if ((align & Alignment.Right) != 0)
    //     {
    //         originX = container.X + hPadding;
    //     } 
    //     if ((align & Alignment.Top) != 0)
    //     {
    //         originY = container.Y;
    //     } else if ((align & Alignment.Bottom) != 0)
    //     {
    //         originY = container.Y + vPadding;
    //     }
    //
    //     return new IntVector2(originX, originY);
    // }

    public void DrawBox()
    {
        var spriteBatch = Game.Services.GetService<SpriteBatch>();
        var stringSize = Font.MeasureString(Text);
        var sizeInTiles = new IntVector2((int)Math.Ceiling(stringSize.X / TileSize.X),
            (int)Math.Ceiling(stringSize.Y / TileSize.Y));

        // Position = FindPosition(Parent.Bounds, Size, BoxAlignment);

        for (int y = 0; y < sizeInTiles.Y + 2; y++)
        {
            for (int x = 0; x < sizeInTiles.X + 2; x++)
            {
                var tileRect = new Rectangle(SpriteLocation * TileSize, TileSize);
                var destinationRect = new Rectangle(Position + new IntVector2(x, y) * TileSize, TileSize);
                if (x == 0 && y == 0)
                {
                    
                }
                else if (x == sizeInTiles.X + 1 && y == 0)
                {
                    tileRect.Location += new Point(TileSize.X * 2, 0);
                }
                else if (y == 0)
                {
                    tileRect.Location += new Point(TileSize.X, 0);
                }
                else if (x == 0 && y == sizeInTiles.Y + 1)
                {
                    tileRect.Location += new Point(0, TileSize.Y * 2);
                }
                else if (x == 0)
                {
                    tileRect.Location += new Point(0, TileSize.Y);
                }
                else if (x == sizeInTiles.X + 1 && y == sizeInTiles.Y + 1)
                {
                    tileRect.Location += TileSize * new IntVector2(2, 2);
                }
                else if (y == sizeInTiles.Y + 1)
                {
                    tileRect.Location += new Point(TileSize.X, TileSize.Y * 2);
                }
                else if (x == sizeInTiles.X + 1)
                {
                    tileRect.Location += new Point(TileSize.X * 2, TileSize.Y);
                }
                else
                {
                    tileRect.Location += TileSize;
                }
                var texture = Game.Content.Load<Texture2D>(SpriteSheet);
                spriteBatch.Draw(texture, destinationRect, tileRect, Color.DarkGray);
            }
        }
        DrawText(spriteBatch);
    }

    protected virtual void DrawText(SpriteBatch spriteBatch)
    {
        var textSize = Font.MeasureString(Text);
        // var textPos = FindPosition(ContentBounds, new IntVector2((int)textSize.X, (int)textSize.Y), TextAlignment);
        var padding = (Size - textSize) / IntVector2.Two;
        var textPos = Position + padding;
        
        spriteBatch.DrawString(Font, Text, textPos, Color.White);
    }

    public override void Draw(GameTime gameTime)
    {
        DrawBox();
        base.Draw(gameTime);
    }
}

[Flags]
public enum Alignment
{
    Left = 1,
    Top = 2,
    TopLeft = 3,
    Right = 4,
    TopRight = 6,
    Bottom = 8,
    BottomLeft = 9,
    BottomRight = 12,
    Center = 16,
    CenterLeft = 17,
    TopCenter = 18,
    CenterRight = 20,
    BottomCenter = 24
}
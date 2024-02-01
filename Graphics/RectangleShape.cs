using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Roguelike.Graphics;

public class RectangleShape : DrawableGameComponent
{
    public Texture2D Pixel;
    public Color BorderColor = Color.LimeGreen;
    public int BorderWidth = 1;
    public Rectangle Bounds;
    
    
    public RectangleShape(Game game) : base(game)
    {
    }

    public RectangleShape(Game game, Rectangle bounds) : base(game)
    {
        Bounds = bounds;
    }
    
    protected override void LoadContent()
    {
        Pixel = new Texture2D(GraphicsDevice, 1, 1, false, SurfaceFormat.Color); 
        Pixel.SetData(new[] { Color.White });
        base.LoadContent();
    }

    public override void Draw(GameTime gameTime)
    {
        var spriteBatch = Game.Services.GetService<SpriteBatch>();
        spriteBatch.Draw(Pixel, new Rectangle(Bounds.X, Bounds.Y, Bounds.Width, BorderWidth), BorderColor); 

        spriteBatch.Draw(Pixel, new Rectangle(Bounds.X, Bounds.Y, BorderWidth, Bounds.Height), BorderColor); 

        spriteBatch.Draw(Pixel, new Rectangle((Bounds.X + Bounds.Width - BorderWidth), 
            Bounds.Y, 
            BorderWidth, 
            Bounds.Height), BorderColor);
        
        spriteBatch.Draw(Pixel, new Rectangle(Bounds.X, 
            Bounds.Y + Bounds.Height - BorderWidth, 
            Bounds.Width, 
            BorderWidth), BorderColor);
        base.Draw(gameTime);
    }

}
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Roguelike.Entity;
using Roguelike.Map;

namespace Roguelike;

public class DrawEngine : DrawableGameComponent
{

    private readonly IntVector2 _tileSize = new(16, 16);
    private const int TopBarHeight = 30;




    public DrawEngine(RoguelikeGame game) : base(game)
    {

    }

    private void DrawSpriteAtLocation(SpriteRepresented sprite, IntVector2 loc, SpriteBatch spriteBatch)
    {
        DrawSpriteAtLocation(sprite, loc, spriteBatch, sprite.Color);
    }

    private void DrawSpriteAtLocation(SpriteRepresented sprite, IntVector2 loc, SpriteBatch spriteBatch, Color color)
    {
        var origin = LocationToScreenCoords(loc);
        DrawSpriteAtCoords(sprite, origin, spriteBatch, color);
    }

    private void DrawSpriteAtCoords(SpriteRepresented sprite, Point coords, SpriteBatch spriteBatch, Color color)
    {
        var destinationRect = new Rectangle(coords, _tileSize);
        var tex = Game.Content.Load<Texture2D>(sprite.SpriteSheet);
        spriteBatch.Draw(tex, destinationRect, sprite.Rectangle, color);
    }

    private Point LocationToScreenCoords(IntVector2 loc)
    {
        return new Point(loc.X * _tileSize.X, loc.Y * _tileSize.Y + TopBarHeight);
    }

    private void DrawDungeon(TileMap map, SpriteBatch spriteBatch)
    {
        for (var i = 0; i < map.Tiles.GetLength(0); i++)
        {
            for (var j = 0; j < map.Tiles.GetLength(1); j++)
            {
                DrawSpriteAtLocation(map.Tiles[i, j], new IntVector2(i, j), spriteBatch);
            }
        }
    }

    private void DrawTopBar(SpriteBatch spriteBatch, Player player)
    {
        var topBarPadding = 6;
        var labelSpacing = 4;
        var elementSpacing = 12;
        var tex = Game.Content.Load<Texture2D>("Graphics/monochrome-transparent_packed");
        var font = Game.Content.Load<SpriteFont>("Fonts/Kenney Mini");

        var dollarLoc = new IntVector2(35, 16);
        spriteBatch.Draw(tex, new Vector2(topBarPadding, topBarPadding),
            new Rectangle(dollarLoc * _tileSize, _tileSize), Color.Gold);
        var moneyFieldEnd = topBarPadding + _tileSize.X + labelSpacing + font.MeasureString($"{player.Money}").X;
        spriteBatch.DrawString(font, $"{player.Money}",
            new Vector2(topBarPadding + _tileSize.X + labelSpacing, topBarPadding), Color.Gold);

        var stairsLoc = new IntVector2(2, 6);
        spriteBatch.Draw(tex, new Vector2(moneyFieldEnd + elementSpacing, topBarPadding),
            new Rectangle(stairsLoc * _tileSize, _tileSize), Color.Gold);
        var levelFieldEnd = moneyFieldEnd + elementSpacing + _tileSize.X + labelSpacing +
                            font.MeasureString($"{player.Location.Z}").X;
        spriteBatch.DrawString(font, $"{player.Location.Z}",
            new Vector2(moneyFieldEnd + elementSpacing + _tileSize.X + labelSpacing, topBarPadding), Color.Gold);

        var heartLoc = new IntVector2(39, 10);
        spriteBatch.Draw(tex, new Vector2(levelFieldEnd + elementSpacing, topBarPadding),
            new Rectangle(heartLoc * _tileSize, _tileSize), Color.Gold);
        spriteBatch.DrawString(font, $"{player.Hp}",
            new Vector2(levelFieldEnd + elementSpacing + _tileSize.X + labelSpacing, topBarPadding), Color.Gold);

    }


    public override void Draw(GameTime gameTime)
    {
        var levelManager = Game.Services.GetService<LevelManager>();
        var map = levelManager.CurrentMap;
        var spriteBatch = new SpriteBatch(Game.GraphicsDevice);
        spriteBatch.Begin();

        DrawDungeon(map, spriteBatch);

        foreach (var entity in levelManager.CurrentLevel.EntitiesOnLevel())
        {
            DrawSpriteAtLocation(entity, entity.Location.To2D, spriteBatch);
        }

        // Draw Player
        var player = Game.Services.GetService<PlayerManager>().Player;
        DrawSpriteAtLocation(player, player.Location.To2D, spriteBatch);

        DrawTopBar(spriteBatch, player);



        var log = Game.Services.GetService<ActivityLog>();
        log.Size = new IntVector2(48, 4);
        log.BoxAlignment = Alignment.BottomCenter;
        log.DrawBox(spriteBatch);


        if (Game.Services.GetService<InputManager>().GameState == InputState.GameOver)
        {
            var gameOverMsg = new DialogBox(Game);
            gameOverMsg.Text = "Game Over";
            gameOverMsg.DrawBox(spriteBatch);
        }


        spriteBatch.End();
    }
    
}
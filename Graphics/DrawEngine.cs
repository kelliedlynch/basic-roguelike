using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Roguelike.Entity;
using Roguelike.Graphics.Layout;
using Roguelike.Map;
using Roguelike.UserInterface;

namespace Roguelike.Graphics;

public class DrawEngine : DrawableGameComponent
{

    private readonly IntVector2 _tileSize = new(16, 16);
    private const int TopBarHeight = 120;
    public List<Container> Containers = new();




    public DrawEngine(RoguelikeGame game) : base(game)
    {

    }

    public override void Initialize()
    {
        BuildLayout();
    }

    public void BuildLayout()
    {
        // var container = new Container(Game);
        // Containers.Add(container);
        // Game.Components.Add(container);
        
        // var box = new DialogBox(Game);
        // box.Text = "test";
        // container.AddElement(box);

        var topBar = new VStackContainer((RoguelikeGame)Game, new Rectangle(0, 0, 200, 400));
        topBar.Sizing = AxisSizing.FixedXFixedY;
        topBar.Outline.BorderWidth = 4;
        topBar.Outline.BorderColor = Color.DarkGray;
        // topBar.ContentAlignment = Alignment.Center;
        
        var moneyDisplay = new VStackContainer((RoguelikeGame)Game);
        moneyDisplay.Sizing = AxisSizing.ShrinkXShrinkY;
        moneyDisplay.Outline.BorderWidth = 3;
        moneyDisplay.Outline.BorderColor = Color.DarkCyan;
        moneyDisplay.DrawOrder = topBar.DrawOrder + 1;
        topBar.AddChild(moneyDisplay);

        var mbox1 = new VStackContainer((RoguelikeGame)Game);
        mbox1.Sizing = AxisSizing.ExpandXExpandY;
        mbox1.MinSize = new IntVector2(100, 100);
        mbox1.Outline.BorderWidth = 2;
        mbox1.Outline.BorderColor = Color.Magenta;
        mbox1.DrawOrder = moneyDisplay.DrawOrder + 1;
        moneyDisplay.AddChild(mbox1);
        
        var mbox2 = new VStackContainer((RoguelikeGame)Game);
        mbox2.Sizing = AxisSizing.ExpandXExpandY;
        mbox2.MinSize = new IntVector2(50, 50);
        mbox2.Outline.BorderWidth = 1;
        mbox2.Outline.BorderColor = Color.LimeGreen;
        mbox2.DrawOrder = mbox1.DrawOrder + 1;
        moneyDisplay.AddChild(mbox2);


        // var moneyBox = new DialogBox(Game);
        // moneyBox.Text = $"$ {Game.Services.GetService<PlayerManager>().Player.Money}";
        // moneyDisplay.AddChild(moneyBox);
        // moneyBox.Outline.BorderWidth = 6;
        // moneyBox.Outline.BorderColor = Color.Red;
        // moneyBox.Sizing = AxisSizing.ExpandXExpandY;
        // var moneyBox2 = new DialogBox(Game);
        // moneyBox2.Text = $"$ {Game.Services.GetService<PlayerManager>().Player.Money} 2";
        // moneyDisplay.AddChild(moneyBox2);
        // moneyBox2.Outline.BorderWidth = 6;
        // moneyBox2.Outline.BorderColor = Color.Pink;
        // moneyBox.Sizing = AxisSizing.ExpandXExpandY;
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

    private void DrawDungeon(DungeonMap map, SpriteBatch spriteBatch)
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
        var hpFieldEnd = levelFieldEnd + elementSpacing + _tileSize.X + labelSpacing +
                         font.MeasureString($"{player.CalculatedAtk}").X;
        spriteBatch.DrawString(font, $"{player.Hp}",
            new Vector2(levelFieldEnd + elementSpacing + _tileSize.X + labelSpacing, topBarPadding), Color.Gold);
        
        var swordLoc = new IntVector2(32, 8);
        spriteBatch.Draw(tex, new Vector2(hpFieldEnd + elementSpacing, topBarPadding),
            new Rectangle(swordLoc * _tileSize, _tileSize), Color.Gold);
        spriteBatch.DrawString(font, $"{player.CalculatedAtk}",
            new Vector2(hpFieldEnd + elementSpacing + _tileSize.X + labelSpacing, topBarPadding), Color.Gold);

    }
    
    public override void Draw(GameTime gameTime)
    {
        var levelManager = Game.Services.GetService<LevelManager>();
        // var map = levelManager.CurrentMap;
        // var spriteBatch = Game.Services.GetService<SpriteBatch>();
        


        // spriteBatch.Begin();

        // DrawDungeon(map, spriteBatch);
        //
        // foreach (var entity in levelManager.CurrentLevel.EntitiesOnLevel())
        // {
        //     DrawSpriteAtLocation(entity, entity.Location.To2D, spriteBatch);
        // }
        //
        // // Draw Player
        // var player = Game.Services.GetService<PlayerManager>().Player;
        // DrawSpriteAtLocation(player, player.Location.To2D, spriteBatch);
        //
        // DrawTopBar(spriteBatch, player);
        //
        //
        //
        // var log = Game.Services.GetService<ActivityLog>();
        // log.Size = new IntVector2(48, 4);
        // log.BoxAlignment = Alignment.BottomCenter;
        // log.DrawBox(spriteBatch);
        //
        //
        // if (Game.Services.GetService<InputManager>().GameState == GameRunningState.GameOver)
        // {
        //     var gameOverMsg = new DialogBox(Game);
        //     gameOverMsg.Text = "Game Over";
        //     gameOverMsg.DrawBox(spriteBatch);
        // }


        // spriteBatch.End();
    }
    
}
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Roguelike.Entity;
using Roguelike.Graphics.Layout;
using Roguelike.Graphics.UserInterface;
using Roguelike.Map;
using Roguelike.UserInterface;
using Roguelike.Utility;

namespace Roguelike.Graphics;

public class DrawEngine : DrawableGameComponent
{

    private readonly IntVector2 _tileSize = new(16, 16);
    private const int TopBarHeight = 120;
    public List<Container> Containers = new();
    public Container MapContainer;



    public DrawEngine(RoguelikeGame game) : base(game)
    {

    }

    protected override void LoadContent()
    {
        BuildLayout();
        base.LoadContent();
    }

    public void BuildLayout()
    {
        var player = Game.Services.GetService<PlayerManager>().Player;
        var screenContainer = new VStackContainer(Game);
        screenContainer.Sizing = AxisSizing.ExpandXExpandY;
        screenContainer.Bounds = GraphicsDevice.Viewport.Bounds;
        screenContainer.ContentAlignment = Alignment.Center;
        Game.Components.Add(screenContainer);

        var topBar = new InfoBar(Game);
        // topBar.Sizing = AxisSizing.ExpandXFixedY;
        // topBar.Size = new IntVector2(0, 48);
        topBar.Debug = true;
        // topBar.TextLabel.Text = $"$ {player.Money}  Lv. {player.Location.Z}  Atk. {player.CalculatedAtk}";
        screenContainer.AddChild(topBar);

        MapContainer = new Container(Game);
        MapContainer.Sizing = AxisSizing.ExpandXExpandY;
        // mapContainer.Sizing = AxisSizing.ExpandXFixedY;
        // mapContainer.Size = new IntVector2(0, 300);
        MapContainer.Debug = true;
        screenContainer.AddChild(MapContainer);
        topBar.DrawOrder = MapContainer.DrawOrder + 1;
        
        var log = Game.Services.GetService<ActivityLog>();
        log.Sizing = AxisSizing.ExpandXFixedY;
        log.Size = new IntVector2(0, 78);
        log.Debug = true;
        log.Reparent(screenContainer);

        var menu = Game.Services.GetService<MenuManager>().InventoryMenu;
        menu.Position = new IntVector2(200, 400);
        menu.Size = new IntVector2(200, 300);
        menu.Debug = true;
        // menu.DrawOrder = MapContainer.DrawOrder + 1000;
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
        return new Point(MapContainer.Bounds.X + loc.X * _tileSize.X, MapContainer.Bounds.Y + loc.Y * _tileSize.Y);
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


        var stairsLoc = new IntVector2(2, 6);

        var heartLoc = new IntVector2(39, 10);

        
        var swordLoc = new IntVector2(32, 8);


    }
    
    public override void Draw(GameTime gameTime)
    {
        var levelManager = Game.Services.GetService<LevelManager>();
        
        var map = levelManager.CurrentMap;
        var spriteBatch = Game.Services.GetService<SpriteBatch>();
        


        // spriteBatch.Begin();

        DrawDungeon(map, spriteBatch);
        
        foreach (var entity in levelManager.CurrentLevel.EntitiesOnLevel())
        {
            DrawSpriteAtLocation(entity, entity.Location.To2D, spriteBatch);
        }
        
        // Draw Player
        var player = Game.Services.GetService<PlayerManager>().Player;
        DrawSpriteAtLocation(player, player.Location.To2D, spriteBatch);
        //
        // DrawTopBar(spriteBatch, player);
        //
        //
        //

        // log.BoxAlignment = Alignment.BottomCenter;
        // log.DrawBox(spriteBatch);
        //
        //
        if (Game.Services.GetService<InputManager>().GameState == GameRunningState.GameOver)
        {
            var gameOverMsg = new DialogBox(Game);
            gameOverMsg.TextLabel.Text = "Game Over";
            Game.Components.Add(gameOverMsg);
        }


        // spriteBatch.End();
    }
    
}
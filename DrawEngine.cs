using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using JetBrains.Annotations;
using Microsoft.Xna.Framework.Input;
using Roguelike.Entity;
using Roguelike.Map;

namespace Roguelike;

public class DrawEngine : DrawableGameComponent
{
    
    private readonly IntVector2 _tileSize = new(16, 16);
    private readonly Rectangle _topBar;
    private const int TopBarHeight = 30;
    private readonly Rectangle _mapWindow;
    private readonly Dictionary<TileType, TextureData> _textures = new();
    private readonly Dictionary<string, Texture2D> _spriteSheets = new();
    


    public DrawEngine(RoguelikeGame game) : base(game)
    {
        // EnemyManager = new EnemyManager(game);
        _topBar = new Rectangle(Point.Zero, new Point(Game.GraphicsDevice.Viewport.Width, TopBarHeight));
        _mapWindow = new Rectangle(new Point(0, TopBarHeight),
            new Point(Game.GraphicsDevice.Viewport.Width, Game.GraphicsDevice.Viewport.Height));
    }

    public override void Initialize()
    {

        base.Initialize();
    }

    protected override void LoadContent()
    {
        // LoadTextures("TileTextures");
        base.LoadContent();
    }

    // private void LoadTextures(string textureDefFile)
    // {
    //     var rootPath = Path.GetFullPath(@"../../../");
    //     var rawTexData = File.ReadAllText($"{rootPath}/{textureDefFile}.json");
    //     var texData = JsonConvert.DeserializeObject<List<TextureData>>(rawTexData);
    //
    //     if (texData is null)
    //     {
    //         Console.WriteLine($"Unable to deserialize texture file {textureDefFile}");
    //         return;
    //     }
    //
    //     foreach (var tex in texData)
    //     {
    //         if (!_spriteSheets.ContainsKey(tex.SpriteSheet))
    //         {
    //             var newSheet = Game.Content.Load<Texture2D>($"Graphics/{tex.SpriteSheet}");
    //             _spriteSheets[tex.SpriteSheet] = newSheet;
    //         }
    //
    //         tex.SpriteSheetTexture = _spriteSheets[tex.SpriteSheet];
    //         var tileRect = new Rectangle(tex.TextureLocationX * _tileSize.X, tex.TextureLocationY * _tileSize.Y,
    //             _tileSize.X, _tileSize.Y);
    //         tex.TileRect = tileRect;
    //         _textures[tex.TileType] = tex;
    //     }
    //
    // }

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

    private void DrawFeatures(TileMap map, SpriteBatch spriteBatch)
    {
        for (var i = 0; i < map.Features.GetLength(0); i++)
        {
            for (var j = 0; j < map.Features.GetLength(1); j++)
            {
                var features = map.Features[i, j];
                foreach (var f in features)
                {
                    DrawSpriteAtLocation(f, new IntVector2(i, j), spriteBatch);
                }
            }
        }
    }

    public void DrawOverlay(SpriteBatch spriteBatch, Player player)
    {
        var tex = Game.Content.Load<Texture2D>("Graphics/monochrome-transparent_packed");
        var loc = new IntVector2(35, 16);
        spriteBatch.Draw(tex, new Vector2(6, 6), new Rectangle(loc * _tileSize, _tileSize), Color.Gold);
        var font = Game.Content.Load<SpriteFont>("Fonts/Kenney Mini");
        spriteBatch.DrawString(font, $"{player.Money}", new Vector2(24, 4), Color.Gold);
    }
    
    
    public override void Draw(GameTime gameTime)
    {
        var man = Game.Services.GetService<MapManager>();
        var map = man.CurrentMap;
        var spriteBatch = new SpriteBatch(Game.GraphicsDevice);
        spriteBatch.Begin();
        
        DrawDungeon(map, spriteBatch);

        DrawFeatures(map, spriteBatch);

        var ent = Game.Services.GetService<EntityManager>().Entities;
        foreach (var entity in ent)
        {
            DrawSpriteAtLocation(entity, entity.Location, spriteBatch);
        }
        
        // Draw Player
        var player = Game.Services.GetService<PlayerManager>().Player;
        DrawSpriteAtLocation(player, player.Location, spriteBatch);
        
        // Draw Enemies
        var eman = Game.Services.GetService<EnemyManager>();
        foreach (var enemy in eman.Enemies[man.CurrentDungeonLevel - 1])
        {
            DrawSpriteAtLocation(enemy, enemy.Location, spriteBatch);
        }

        DrawOverlay(spriteBatch, player);
        
        spriteBatch.End();
    }


    public class TextureData
    {
        [UsedImplicitly]
        public TileType TileType;
        [UsedImplicitly]
        public string SpriteSheet;
        [UsedImplicitly]
        public int TextureLocationX;
        [UsedImplicitly]
        public int TextureLocationY;

        private string _colorName;

        [UsedImplicitly]
        public string ColorName
        {
            get => _colorName;
            set
            {
                _colorName = value;
                LoadColor();
            }
        }
        public Color Color;
        public Rectangle TileRect = Rectangle.Empty;
        public Texture2D SpriteSheetTexture;

        private void LoadColor()
        {
            try
            {
                Color = (Color)typeof(Color).GetProperty(ColorName)!.GetValue(null, null)!;
            }
            catch (NullReferenceException)
            {
                Color = Color.Black;
                Console.WriteLine($"Color \"{ColorName}\" not found. Check tile definition file.");
            }
        }
    }

}
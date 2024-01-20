using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using JetBrains.Annotations;
using Microsoft.Xna.Framework.Input;

namespace Roguelike;

public class DrawEngine : DrawableGameComponent
{
    
    private readonly IntVector2 _tileSize = new(16, 16);
    private readonly Dictionary<TileType, TextureData> _textures = new();
    private readonly Dictionary<string, Texture2D> _spriteSheets = new();
    


    public DrawEngine(RoguelikeGame game) : base(game)
    {
        // EnemyManager = new EnemyManager(game);
        
    }

    public override void Initialize()
    {

        base.Initialize();
    }

    protected override void LoadContent()
    {
        LoadTextures("TileTextures");
        base.LoadContent();
    }

    private void LoadTextures(string textureDefFile)
    {
        var rootPath = Path.GetFullPath(@"../../../");
        var rawTexData = File.ReadAllText($"{rootPath}/{textureDefFile}.json");
        var texData = JsonConvert.DeserializeObject<List<TextureData>>(rawTexData);

        if (texData is null)
        {
            Console.WriteLine($"Unable to deserialize texture file {textureDefFile}");
            return;
        }

        foreach (var tex in texData)
        {
            if (!_spriteSheets.ContainsKey(tex.SpriteSheet))
            {
                var newSheet = Game.Content.Load<Texture2D>($"Graphics/{tex.SpriteSheet}");
                _spriteSheets[tex.SpriteSheet] = newSheet;
            }

            tex.SpriteSheetTexture = _spriteSheets[tex.SpriteSheet];
            var tileRect = new Rectangle(tex.TextureLocationX * _tileSize.X, tex.TextureLocationY * _tileSize.Y,
                _tileSize.X, _tileSize.Y);
            tex.TileRect = tileRect;
            _textures[tex.TileType] = tex;
        }

    }

    // public override void Update(GameTime gameTime)
    // {
    //     var player = Game.Services.GetService<PlayerManager>().Player;
    //     var map = Game.Services.GetService<MapManager>().CurrentMap;
    //     var keyboard = Keyboard.GetState();
    //     if (!_keyIsPressed)
    //     {
    //         if (keyboard.GetPressedKeys().Length > 0)
    //         {
    //             _keyIsPressed = true;
    //         }
    //
    //         var destination = player.Location;
    //         if (keyboard.IsKeyDown(Keys.Up))
    //         {
    //             destination += Direction.Up;
    //         }
    //         else if (keyboard.IsKeyDown(Keys.Down))
    //         {
    //             destination += Direction.Down;
    //         }
    //         else if (keyboard.IsKeyDown(Keys.Left))
    //         {
    //             destination += Direction.Left;
    //         }
    //         else if (keyboard.IsKeyDown(Keys.Right))
    //         {
    //             destination += Direction.Right;
    //         }
    //
    //         if (destination != player.Location)
    //         {
    //             var path = player.Pathfinder.FindPath(map, player.Location, destination);
    //             if (path is not null && path.Count > 0)
    //             {
    //                 player.Location = destination;
    //             }
    //         }
    //     }
    //     else
    //     {
    //         if (keyboard.GetPressedKeys().Length == 0)
    //         {
    //             _keyIsPressed = false;
    //         }
    //     }
    //
    //     
    // }
    
    
    public override void Draw(GameTime gameTime)
    {
        var map = Game.Services.GetService<MapManager>().CurrentMap;
        var spriteBatch = new SpriteBatch(Game.GraphicsDevice);
        spriteBatch.Begin();
        // Draw Dungeon
        for (var i = 0; i < map.Tiles.GetLength(0); i++)
        {
            for (var j = 0; j < map.Tiles.GetLength(1); j++)
            {
                var tileOrigin = new Vector2(i * _tileSize.X, j * _tileSize.Y);
                var tile = map.GetTileAt(new IntVector2(i, j));
                var tileData = _textures[tile.Type];
                spriteBatch.Draw(tileData.SpriteSheetTexture, tileOrigin, tileData.TileRect, tileData.Color);
            }
        }
        
        // Draw Features
        for (var i = 0; i < map.Features.GetLength(0); i++)
        {
            for (var j = 0; j < map.Features.GetLength(1); j++)
            {
                var features = map.Features[i, j];
                foreach (var f in features)
                {
                    var featureSpriteOrigin = new IntVector2(f.SpriteLocation.X * _tileSize.X, f.SpriteLocation.Y * _tileSize.Y);
                    var featureSpriteRect = new Rectangle(featureSpriteOrigin.X, featureSpriteOrigin.Y, _tileSize.X, _tileSize.Y);
                    var featureDestinationRect = new Rectangle(f.Location.X * _tileSize.X, f.Location.Y * _tileSize.Y,
                        _tileSize.X, _tileSize.Y);
                    var featureTexture = Game.Content.Load<Texture2D>(f.SpriteSheet);
                    spriteBatch.Draw(featureTexture, featureDestinationRect, featureSpriteRect, f.Color);
                }

            }
        }
        
        // Draw Player
        var player = Game.Services.GetService<PlayerManager>().Player;
        var destinationRect =
            new Rectangle(player.Location.X * _tileSize.X, player.Location.Y * _tileSize.Y, _tileSize.X, _tileSize.Y);
        var spriteRect = new Rectangle(player.SpriteLocation.X * _tileSize.X, player.SpriteLocation.Y * _tileSize.Y, _tileSize.X,
            _tileSize.Y);
        var spriteSheet = Game.Content.Load<Texture2D>(player.SpriteSheet);
        spriteBatch.Draw(spriteSheet, destinationRect, spriteRect, player.Color);
        
        // Draw Enemies
        var eman = Game.Services.GetService<EnemyManager>();
        foreach (var enemy in eman.Enemies)
        {
            var enemyDestinationRect =
                new Rectangle(enemy.Location.X * _tileSize.X, enemy.Location.Y * _tileSize.Y, _tileSize.X, _tileSize.Y);
            var enemySpriteRect = new Rectangle(enemy.SpriteLocation.X * _tileSize.X, enemy.SpriteLocation.Y * _tileSize.Y, _tileSize.X,
                _tileSize.Y);
            var enemySpriteSheet = Game.Content.Load<Texture2D>(enemy.SpriteSheet);
            spriteBatch.Draw(enemySpriteSheet, enemyDestinationRect, enemySpriteRect, enemy.Color);
        }
        
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
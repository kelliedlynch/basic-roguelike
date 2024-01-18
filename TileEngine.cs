using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using JetBrains.Annotations;
using Microsoft.Xna.Framework.Input;

namespace Roguelike;

public class TileEngine
{

    private TileMap _tileMap;
    
    private readonly IntVector2 _tileSize = new(16, 16);
    private readonly Random _random = new();
    private readonly Dictionary<TileType, TextureData> _textures = new();
    private readonly ContentManager _content;
    private readonly Dictionary<string, Texture2D> _spriteSheets = new();
    private readonly Player _player;
    private bool _keyIsPressed;


    public TileEngine(IServiceProvider serviceProvider)
    {

        _content = new ContentManager(serviceProvider, "Content");

        LoadTextures("TileTextures");

        _player = new Player();
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
                var newSheet = _content.Load<Texture2D>($"Graphics/{tex.SpriteSheet}");
                _spriteSheets[tex.SpriteSheet] = newSheet;
            }

            tex.SpriteSheetTexture = _spriteSheets[tex.SpriteSheet];
            var tileRect = new Rectangle(tex.TextureLocationX * _tileSize.X, tex.TextureLocationY * _tileSize.Y,
                _tileSize.X, _tileSize.Y);
            tex.TileRect = tileRect;
            _textures[tex.TileType] = tex;
        }

    }

    public void ChangeMap(MapGenerator generator)
    {
        _tileMap = generator.GenerateDungeonMap();
        SpawnInPlayer();
    }

    public void SpawnInPlayer()
    {
        var t = _random.Next(0, 7);
        var adj = _tileMap.GetAdjacentTiles(new IntVector2(_tileMap.StairsUp.Location.X, _tileMap.StairsUp.Location.Y), 2);
        _player.Location = adj[t].Location;
    }

    public void Update(GameTime gameTime)
    {
        var keyboard = Keyboard.GetState();
        if (!_keyIsPressed)
        {
            if (keyboard.GetPressedKeys().Length > 0)
            {
                _keyIsPressed = true;
            }

            var destination = _player.Location;
            if (keyboard.IsKeyDown(Keys.Up))
            {
                destination += Direction.Up;
            }
            else if (keyboard.IsKeyDown(Keys.Down))
            {
                destination += Direction.Down;
            }
            else if (keyboard.IsKeyDown(Keys.Left))
            {
                destination += Direction.Left;
            }
            else if (keyboard.IsKeyDown(Keys.Right))
            {
                destination += Direction.Right;
            }

            if (destination != _player.Location)
            {
                var path = _player.Pathfinder.FindPath(_tileMap, _player.Location, destination);
                if (path is not null && path.Count > 0)
                {
                    _player.Location = destination;
                }
            }
        }
        else
        {
            if (keyboard.GetPressedKeys().Length == 0)
            {
                _keyIsPressed = false;
            }
        }

        
    }
    
    
    public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
        // Draw Dungeon
        for (var i = 0; i < _tileMap.Tiles.GetLength(0); i++)
        {
            for (var j = 0; j < _tileMap.Tiles.GetLength(1); j++)
            {
                var tileOrigin = new Vector2(i * _tileSize.X, j * _tileSize.Y);
                var tile = _tileMap.GetTileAt(new IntVector2(i, j));
                var tileData = _textures[tile.Type];
                spriteBatch.Draw(tileData.SpriteSheetTexture, tileOrigin, tileData.TileRect, tileData.Color);
            }
        }
        
        // Draw Features
        for (var i = 0; i < _tileMap.Features.GetLength(0); i++)
        {
            for (var j = 0; j < _tileMap.Features.GetLength(1); j++)
            {
                var features = _tileMap.Features[i, j];
                foreach (var f in features)
                {
                    var featureSpriteOrigin = new IntVector2(f.SpriteLocation.X * _tileSize.X, f.SpriteLocation.Y * _tileSize.Y);
                    var featureSpriteRect = new Rectangle(featureSpriteOrigin.X, featureSpriteOrigin.Y, _tileSize.X, _tileSize.Y);
                    var featureDestinationRect = new Rectangle(f.Location.X * _tileSize.X, f.Location.Y * _tileSize.Y,
                        _tileSize.X, _tileSize.Y);
                    var featureTexture = _content.Load<Texture2D>(f.SpriteSheet);
                    spriteBatch.Draw(featureTexture, featureDestinationRect, featureSpriteRect, Color.Chartreuse);
                }

            }
        }
        
        // Draw Player
        var destinationRect =
            new Rectangle(_player.Location.X * _tileSize.X, _player.Location.Y * _tileSize.Y, _tileSize.X, _tileSize.Y);
        var spriteRect = new Rectangle(_player.SpriteLocation.X * _tileSize.X, _player.SpriteLocation.Y * _tileSize.Y, _tileSize.X,
            _tileSize.Y);
        var spriteSheet = _content.Load<Texture2D>(_player.SpriteSheet);
        spriteBatch.Draw(spriteSheet, destinationRect, spriteRect, Color.Aqua);
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

public struct Direction
{
    public static IntVector2 Up { get; } = new(0, -1);
    public static IntVector2 Down { get; } = new(0, 1);
    public static IntVector2 Left { get; } = new(-1, 0);
    public static IntVector2 Right { get; } = new(1, 0);
}

public class ValidLocationNotFoundException : SystemException
{
    public ValidLocationNotFoundException(string message)
    {
        Console.WriteLine(message);
    }
}
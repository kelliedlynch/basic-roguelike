using System;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Roguelike.Content.Entity.Creature;

namespace Roguelike;

public class EnemyManager : DrawableGameComponent
{
    public List<Creature> Enemies = new();
    public Player Player;
    public TileMap Map;
    private Random _random = new();

    public EnemyManager(RoguelikeGame game) : base(game)
    {
        game.BeginGame += BeginGame;
    }

    public override void Initialize()
    {

        base.Initialize();
    }

    public void BeginGame(object sender, EventArgs e)
    {
        Player = Game.Services.GetService<PlayerManager>().Player;
        Map = Game.Services.GetService<DrawEngine>().TileMap;
        Enemies = new List<Creature>();
        PlaceEnemies();
    }

    public void PlaceEnemies()
    {
        do
        {
            SpawnNewEnemy();
        } while (!EnemyCapReached());
    }
    
    private void SpawnNewEnemy()
    {
        var map = Game.Services.GetService<DrawEngine>().TileMap;
        var enemy = new Creature();
        var loc = FindValidSpawnLocation(map, enemy);
        if (loc is null) return;
        enemy.Location = loc.Value;
        Enemies.Add(enemy);
    }

    private bool EnemyCapReached()
    {
        return Enemies.Count > 10;
    }

    private IntVector2? FindValidSpawnLocation(TileMap map, Creature enemy)
    {
        var player = Game.Services.GetService<PlayerManager>().Player;
        var pathfinder = new Pathfinder();
        var i = 0;
        do
        {
            var x = _random.Next(map.Width);
            var y = _random.Next(map.Height);
            var tile = map.GetTileAt(new IntVector2(x, y));
            if (tile.Type is TileType.Void or TileType.Wall || tile.Location == player.Location) continue;
            var path = pathfinder.FindPath(map, tile.Location, player.Location);
            if (path == null)
            {
                //TODO: NEED TO FIND OUT WHY PATH IS SOMETIMES NULL
                Console.WriteLine($"Could not find path from {tile.Location} to {player.Location}");
            }
            if (path.Count < 7)
            {
                i++;
                continue;
            }

            return tile.Location;
        } while (i < 200);

        return null;
    }

    // public override void Draw(GameTime gameTime)
    // {
    //     var spriteBatch = new SpriteBatch(Game.GraphicsDevice);
    //     spriteBatch.Begin();
    //
    //     foreach (var enemy in Enemies)
    //     {
    //         var destinationRect =
    //             new Rectangle(enemy.Location.X * 16, enemy.Location.Y * 16, 16, 16);
    //         var spriteRect = new Rectangle(enemy.SpriteLocation.X * 16, enemy.SpriteLocation.Y * 16, 16,
    //             16);
    //         var spriteSheet = Game.Content.Load<Texture2D>(Player.SpriteSheet);
    //         spriteBatch.Draw(spriteSheet, destinationRect, spriteRect, Color.Aqua);
    //     }
    //     
    //     spriteBatch.End();
    //     base.Draw(gameTime);
    // }
}
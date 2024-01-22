using System;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Roguelike.Entity;
using Roguelike.Map;

namespace Roguelike;

public class EnemyManager : DrawableGameComponent
{
    public List<List<Creature>> Enemies = new();
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
        Enemies = new List<List<Creature>>();
        Enemies.Add(new List<Creature>());
        PlaceEnemies();
    }

    private void RunSpawnCycle()
    {
        
    }
    
    public void PlaceEnemies()
    {
        var dungeonLevel = Game.Services.GetService<MapManager>().CurrentDungeonLevel;
        do
        {
            SpawnNewEnemy(dungeonLevel);
        } while (!EnemyCapReached(dungeonLevel));
    }
    
    private void SpawnNewEnemy(int dungeonLevel)
    {
        var map = Game.Services.GetService<MapManager>().CurrentMap;
        var enemy = new Creature();
        var shouldAddEnemy = true;
        try
        {
            var loc = FindValidSpawnLocation(map, enemy);
            enemy.Location = loc;
        }
        catch (PathfinderException e)
        {
            // ENEMY WAS PLACED IN INVALID POSITION (NO PATH TO PLAYER)
            enemy.Color = Color.Crimson;
        }
        catch (NoValidLocationException e)
        {
            // NO VALID LOCATION EXISTS FOR ENEMY
            shouldAddEnemy = false;
        }
        finally
        {
            if (shouldAddEnemy)
            {
                enemy.DungeonLevel = dungeonLevel;
                AddEnemyToWorld(enemy);
            }
        }
    }

    private void AddEnemyToWorld(Creature enemy)
    {
        Enemies[enemy.DungeonLevel - 1].Add(enemy);
        enemy.CreatureWasDestroyed += RemoveEnemyFromWorld;
    }

    private void RemoveEnemyFromWorld(object sender, EventArgs args)
    {
        var enemy = (Creature)sender;
        var a = (DestroyEventArgs)args;
        Enemies[enemy.DungeonLevel - 1].Remove(enemy);
        foreach (var item in a.ItemsDropped)
        {
            item.Location = enemy.Location;
            Game.Services.GetService<EntityManager>().AddEntityToWorld(item);
        }
    }

    private bool EnemyCapReached(int dungeonLevel)
    {
        return Enemies[dungeonLevel - 1].Count > 10;
    }

    private IntVector2 FindValidSpawnLocation(TileMap map, Creature enemy)
    {
        var attempts = 200;
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
                throw new PathfinderException($"Could not find path from {tile.Location} to {player.Location}");
            }
            if (path.Count < 7)
            {
                i++;
                continue;
            }

            return tile.Location;
        } while (i < attempts);
        throw new NoValidLocationException(map, enemy, $"Could not find valid spawn location after {attempts} attempts");
    }

}

public class PathfinderException : Exception
{
    public PathfinderException(string message) : base(message)
    {
    }
}

public class NoValidLocationException : Exception
{
    public TileMap Map;
    public Entity.Entity Entity;
    
    public NoValidLocationException(TileMap map, Entity.Entity entity, string message) : base(message)
    {
        Map = map;
        Entity = entity;
        Console.WriteLine($"No valid location for entity {entity}");
    }
}
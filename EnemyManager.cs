using System;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Roguelike.Entity;
using Roguelike.Map;

namespace Roguelike;

public class EnemyManager : RoguelikeGameManager
{
    public List<List<Creature>> Enemies = new();
    private Random _random = new();

    public EnemyManager(RoguelikeGame game) : base(game)
    {
    }

    public List<Creature> EnemiesOnLevel(int level)
    {
        var l = new List<Creature>();
        if (Enemies.Count >= level)
        {
            l.AddRange(Enemies[level - 1]);
        }

        return l;
    }

    protected override void OnConnectManagers(object sender, EventArgs e)
    {
        // This event happens when all the manager classes are loaded. This is where we
        // subscribe to events from other managers.
        Game.Services.GetService<MapManager>().DungeonLevelAdded += OnDungeonLevelAdded;
        Game.Services.GetService<PlayerManager>().AdvanceTurn += OnAdvanceTurn;
        base.OnConnectManagers(sender, e);
    }
    
    protected override void OnBeginGame(object sender, EventArgs e)
    {
        // This event happens upon beginning a new game. Managers are loaded/triggered in this order:
        // Player -> Map -> Enemy -> Entity -> Input
        Enemies = new List<List<Creature>>();
        RunSpawnCycle();
        base.OnBeginGame(sender, e);
    }

    public void OnDungeonLevelAdded(object sender, EventArgs e)
    {
        Enemies.Add(new List<Creature>());
    }

    public void OnAdvanceTurn(object sender, EventArgs e)
    {
        RunSpawnCycle();
    }

    private void RunSpawnCycle()
    {
        PlaceEnemies();
    }
    
    public void PlaceEnemies()
    {
        var mapman = Game.Services.GetService<MapManager>();
        var dungeonLevel = Game.Services.GetService<MapManager>().CurrentDungeonLevel;
        while (!EnemyCapReached(dungeonLevel))
        {
            SpawnNewEnemy(dungeonLevel);
        } 
    }
    
    private void SpawnNewEnemy(int dungeonLevel)
    {
        var map = Game.Services.GetService<MapManager>().CurrentMap;
        var enemy = new Creature();
        var shouldAddEnemy = true;
        try
        {
            var loc = FindValidSpawnLocation(map, enemy);
            enemy.Location = new IntVector3(loc.X, loc.Y, dungeonLevel);
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
                // enemy.DungeonLevel = dungeonLevel;
                // enemy.WorldLocation = new IntVector3()
                AddEnemyToLevel(enemy);
            }
        }
    }

    private void AddEnemyToLevel(Creature enemy)
    {
        while (Enemies.Count < enemy.Location.Z)
        {
            Enemies.Add(new List<Creature>());
        }
        
        Enemies[enemy.Location.Z - 1].Add(enemy);
        enemy.CreatureWasDestroyed += RemoveEnemyFromWorld;
    }

    private void RemoveEnemyFromWorld(object sender, EventArgs args)
    {
        var enemy = (Creature)sender;
        var a = (DestroyEventArgs)args;
        Enemies[enemy.Location.Z - 1].Remove(enemy);
        foreach (var item in a.ItemsDropped)
        {
            item.Location = enemy.Location;
            Game.Services.GetService<EntityManager>().AddEntityToWorld(item);
        }
    }

    private bool EnemyCapReached(int dungeonLevel)
    {
        return EnemiesOnLevel(dungeonLevel).Count > 10;
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
            var path = pathfinder.FindPath(map, tile.Location.To2D, player.Location.To2D);
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

            return tile.Location.To2D;
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
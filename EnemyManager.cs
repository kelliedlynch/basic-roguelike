using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Roguelike.Entity;
using Roguelike.Entity.Creature;
using Roguelike.Map;

namespace Roguelike;

public class EnemyManager : RoguelikeGameManager
{
    private List<List<Creature>> _enemies = new();
    private Random _random = new();

    public EnemyManager(RoguelikeGame game) : base(game)
    {
    }

    public List<Creature> EnemiesOnLevel(int level)
    {
        var l = new List<Creature>();
        if (_enemies.Count >= level)
        {
            l.AddRange(_enemies[level - 1]);
        }

        return l;
    }

    public void InitializeEnemies()
    {
        _enemies.Clear();
        PopulateLevel(1);
    }

    public void RunSpawnCycle()
    {
        
    }
    
    public void PopulateLevel(int level)
    {
        // Fill the dungeon level with randomly-placed monsters up to the cap
        // Typically run when a level is first generated.
        // Currently can only be run after player is placed on level, but I need to change that
        while (_enemies.Count < level)
        {
            _enemies.Add(new List<Creature>());
        }

        while (!EnemyCapReached(level))
        {
            SpawnNewEnemy(level);
        } 
    }
    
    private void SpawnNewEnemy(int dungeonLevel)
    {
        var enemy = new Creature();
        var shouldAddEnemy = true;
        try
        {
            var loc = FindValidSpawnLocation(MapManager.CurrentMap, enemy);
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
                AddEnemyToLevel(enemy);
            }
        }
    }

    private void AddEnemyToLevel(Creature enemy)
    {
        while (_enemies.Count < enemy.Location.Z)
        {
            _enemies.Add(new List<Creature>());
        }
        
        _enemies[enemy.Location.Z - 1].Add(enemy);
        enemy.OnLogEvent += ActivityLog.LogEvent;
        enemy.CreatureWasDestroyed += RemoveEnemyFromWorld;
    }

    private void RemoveEnemyFromWorld(object sender, EventArgs args)
    {
        var enemy = (Creature)sender;
        var a = (DestroyEventArgs)args;
        _enemies[enemy.Location.Z - 1].Remove(enemy);
        foreach (var item in a.ItemsDropped)
        {
            item.Location = enemy.Location;
            EntityManager.AddEntityToWorld(item);
        }
    }

    public void QueueEnemyAttacks()
    {
        foreach (var enemy in EnemiesOnLevel(MapManager.CurrentDungeonLevel))
        {
            var adjacent = MapManager.CurrentMap.GetAdjacentTiles(enemy.Location.To2D, 2);
            foreach (var a in adjacent)
            {
                if (a.Location != Player.Location) continue;
                var args = new AttackEventArgs(enemy, Player);
                TurnManager.QueueAttack(args);
                break;
            }

        }
    }

    public void QueueEnemyMoves()
    {
        foreach (var enemy in EnemiesOnLevel(MapManager.CurrentDungeonLevel))
        {
            if (enemy.CanSeeEntity(MapManager.CurrentMap, Player))
            {
                var path = enemy.Pathfinder.FindPath(MapManager.CurrentMap, enemy.Location.To2D, Player.Location.To2D);
                if (path.Count <= 1) continue;
                var tile = path.Pop();
                var args = new MoveEventArgs(enemy, enemy.Location.To2D, tile.Location.To2D);
                TurnManager.QueueMove(args);
            }
        }
    }

    private bool EnemyCapReached(int dungeonLevel)
    {
        return EnemiesOnLevel(dungeonLevel).Count > 10;
    }

    private IntVector2 FindValidSpawnLocation(TileMap map, Creature enemy)
    {
        var attempts = 200;
        var pathfinder = new Pathfinder();
        var i = 0;
        do
        {
            var x = _random.Next(map.Width);
            var y = _random.Next(map.Height);
            var tile = map.GetTileAt(new IntVector2(x, y));
            if (tile.Type is TileType.Void or TileType.Wall || tile.Location == Player.Location) continue;
            var path = pathfinder.FindPath(map, tile.Location.To2D, Player.Location.To2D);
            if (path == null)
            {
                throw new PathfinderException($"Could not find path from {tile.Location} to {Player.Location}");
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
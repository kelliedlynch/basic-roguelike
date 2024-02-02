using System;
using Microsoft.Xna.Framework;
using Roguelike.Entity.Creature;
using Roguelike.Map;
using Roguelike.Utility;

namespace Roguelike;

public class EnemyManager : RoguelikeGameManager
{
    private readonly Random _random = new();

    public EnemyManager(RoguelikeGame game) : base(game)
    {
    }

    public void InitializeEnemies()
    {
        PopulateLevel(1);
    }
    
    public void PopulateLevel(int level)
    {
        // Fill the dungeon level with randomly-placed monsters up to the cap
        // Typically run when a level is first generated.

        while (!EnemyCapReached(level))
        {
            SpawnNewEnemy(level);
        } 
    }

    public void RunSpawnCycle()
    {
        
    }
    
    private void SpawnNewEnemy(int dungeonLevel)
    {
        var enemy = new Goblin();
        var shouldAddEnemy = true;
        try
        {
            var loc = FindValidSpawnLocation(LevelManager.CurrentLevel, enemy);
            enemy.Location = new IntVector3(loc.X, loc.Y, dungeonLevel);
        }
        catch (PathfinderException e)
        {
            // ENEMY WAS PLACED IN INVALID POSITION (NO PATH TO PLAYER)
            enemy.Color = Color.Crimson;
            Console.WriteLine(e.Message);
        }
        catch (NoValidLocationException e)
        {
            // NO VALID LOCATION EXISTS FOR ENEMY
            shouldAddEnemy = false;
            Console.WriteLine(e.Message);
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
        LevelManager.PlaceEntity(enemy);
        enemy.EntityWasDestroyed += LevelManager.OnDestroyEntity;
        enemy.OnLogEvent += ActivityLog.LogEvent;
    }

    public void ProcessEnemyAttacks()
    {
        foreach (var enemy in LevelManager.CurrentLevel.EnemiesOnLevel())
        {
            if (!enemy.Ready) continue;
            var adjacent = LevelManager.CurrentMap.GetAdjacentTiles(enemy.Location.To2D);
            foreach (var a in adjacent)
            {
                if (a.Location != Player.Location) continue;
                enemy.AttackEntity(Player);
                enemy.Ready = false;
                break;
            }

        }
    }

    public void ProcessEnemyMoves()
    {
        foreach (var enemy in LevelManager.CurrentLevel.EnemiesOnLevel())
        {
            if (!enemy.Ready) continue;
            if (enemy.CanSeeEntity(LevelManager.CurrentLevel, Player))
            {
                var path = enemy.Pathfinder.FindPath(LevelManager.CurrentLevel, enemy.Location.To2D, Player.Location.To2D);
                if (path is null || path.Count <= 1) continue;
                var tile = path.Pop();
                LevelManager.MoveEntity(enemy, tile.Location);
                enemy.Ready = false;
            }
        }
    }

    public void EndTurn()
    {
        foreach (Creature enemy in LevelManager.CurrentLevel.EnemiesOnLevel())
        {
            enemy.Ready = true;
        }
    }

    private bool EnemyCapReached(int dungeonLevel)
    {
        return LevelManager.GetDungeonLevel(dungeonLevel).EnemiesOnLevel().Count > 10;
    }

    private IntVector2 FindValidSpawnLocation(DungeonLevel level, Creature enemy)
    {
        var attempts = 200;
        var pathfinder = new Pathfinder
        {
            CreaturesBlockPath = false
        };
        var i = 0;
        do
        {
            var tile = level.Map.RandomFloorTile();
            if (tile.Location == Player.Location) continue;
            var path = pathfinder.FindPath(level, tile.Location.To2D, Player.Location.To2D);
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
        throw new NoValidLocationException(level.Map, enemy, $"Could not find valid spawn location after {attempts} attempts");
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
    public DungeonMap Map;
    public Entity.Entity Entity;
    
    public NoValidLocationException(DungeonMap map, Entity.Entity entity, string message) : base(message)
    {
        Map = map;
        Entity = entity;
        Console.WriteLine($"No valid location for entity {entity}");
    }
}
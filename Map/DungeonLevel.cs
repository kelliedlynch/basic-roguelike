using System;
using System.Collections.Generic;
using Roguelike.Entity;
using Roguelike.Entity.Creature;
using Roguelike.Entity.Feature;
using Roguelike.Entity.Item;
using Roguelike.Utility;

namespace Roguelike.Map;

public class DungeonLevel : IDungeonLevel
{
    public int LevelNumber => Map.LevelNumber;
    public readonly DungeonMap Map;
    private readonly List<Entity.Entity>[,] _entities;


    public DungeonLevel(DungeonMap map)
    {
        Map = map;
        _entities = new List<Entity.Entity>[map.Width, map.Height];
        for (int i = 0; i < map.Width; i++)
        {
            for (int j = 0; j < map.Height; j++)
            {
                _entities[i, j] = new List<Entity.Entity>();
            }
        }
    }
    
    void IDungeonLevel.PlaceEntity(Entity.Entity entity) => PlaceEntity(entity);
    private void PlaceEntity(Entity.Entity entity)
    {
        if (entity.Location.Z != LevelNumber)
        {
            throw new InvalidDungeonLevelException($"Called wrong level's PlaceEntity. This level: {LevelNumber}, desired Z level: {entity.Location.Z}");
        }
        _entities[entity.Location.X, entity.Location.Y].Add(entity);
    }

    void IDungeonLevel.RemoveEntity(Entity.Entity entity) => RemoveEntity(entity);
    private void RemoveEntity(Entity.Entity entity)
    {
        if (_entities[entity.Location.X, entity.Location.Y].Contains(entity))
        {
            _entities[entity.Location.X, entity.Location.Y].Remove(entity);
        }
        else
        {
            throw new EntityNotFoundException($"Entity {entity.Name} not found on dungeon level {LevelNumber}, cannot remove");
        }
    }

    public List<T> EntityTypeAt<T>(IntVector2 loc)
    {
        return EntityTypeAt<T>(loc.X, loc.Y);
    }
    
    public List<T> EntityTypeAt<T>(int x, int y)
    {
        var entities = new List<T>();
        foreach (var entity in _entities[x, y])
        {
            if (entity is T desiredEntity)
            {
                entities.Add(desiredEntity);
            }
        }
        return entities;
    }
    
    public List<Money> MoneyAt(int x, int y)
    {
        return EntityTypeAt<Money>(x, y);
    }
    
    public List<Item> ItemsAt(int x, int y)
    {
        return EntityTypeAt<Item>(x, y);
    }
    
    public List<Creature> EnemiesAt(int x, int y)
    {
        // TODO: this will need to check for hostile creatures, when that's implemented
        var creatures = new List<Creature>();
        foreach (var entity in _entities[x, y])
        {
            if (entity is not Player && entity is Creature creature)
            {
                creatures.Add(creature);
            }
        }

        return creatures;
    }

    public List<Creature> EnemiesOnLevel()
    {
        // TODO: this will need to check for hostile creatures, when that's implemented
        var enemies = new List<Creature>();
        foreach (var l in _entities)
        {
            foreach (var entity in l)
            {
                if (entity is not Player && entity is Creature creature)
                {
                    enemies.Add(creature);
                }
            }
        }

        return enemies;
    }
    
    public List<Creature> CreaturesAt(int x, int y)
    {
        return EntityTypeAt<Creature>(x, y);
    }
    
    public List<Portal> PortalsAt(int x, int y)
    {
        return EntityTypeAt<Portal>(x, y);
    }

    public List<Entity.Entity> EntitiesAt(int x, int y)
    {
        return _entities[x, y];
    }
    
    public List<Entity.Entity> EntitiesOnLevel()
    {
        var entities = new List<Entity.Entity>();
        foreach (var l in _entities)
        {
            entities.AddRange(l);
        }

        return entities;
    }
}

public interface IDungeonLevel
{
    void PlaceEntity(Entity.Entity entity);
    void RemoveEntity(Entity.Entity entity);

}

public class EntityNotFoundException : Exception
{
    public EntityNotFoundException(string msg) : base(msg)
    {
        
    }
}
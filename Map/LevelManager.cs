using System;
using System.Collections.Generic;
using Roguelike.Entity;
using Roguelike.Entity.Feature;
using Roguelike.Entity.Item;
using Roguelike.Event;

namespace Roguelike.Map;

public class LevelManager : RoguelikeGameManager, IDungeonLevel
{
    public DungeonMap CurrentMap => _levels[_dungeonLevelIndex].Map;
    public DungeonLevel CurrentLevel => _levels[_dungeonLevelIndex];
    
    private int _dungeonLevelIndex;
    public int CurrentLevelNumber
    {
        get => _dungeonLevelIndex + 1;
        private set => _dungeonLevelIndex = value - 1;
    }

    private readonly List<DungeonLevel> _levels = new();

    private readonly Random _random = new();
    
    public LevelManager(RoguelikeGame game) : base(game)
    {
    }
    
    public void InitializeLevels()
    {
        _levels.Clear();
        _dungeonLevelIndex = 0;
        AddDungeonLevel();
       
        Player.Location = new IntVector3(CurrentMap.RandomAdjacentTile(CurrentMap.StairsUp.Location.To2D).Location.To2D, CurrentLevelNumber);
        PlaceEntity(Player);
    }
    
    private void AddDungeonLevel()
    {
        var generator = new MapGenerator();
        var newLevelNumber = _levels.Count + 1;
        var map = generator.GenerateDungeonMap(newLevelNumber);
        _levels.Add(new DungeonLevel(map));
        
        
        if (newLevelNumber > 1)
        {
            var prevLevel = newLevelNumber - 1;
            _levels[prevLevel - 1].Map.StairsDown.LinkedPortal = map.StairsUp;
            map.StairsUp.LinkedPortal = _levels[prevLevel - 1].Map.StairsDown;
        }
        PlaceEntity(map.StairsUp);
        PlaceEntity(map.StairsDown);
        SpawnItems(_levels[^1]);
    }

    private void SpawnItems(DungeonLevel level)
    {
        for (int i = 0; i < 10; i++)
        {
            var a = _random.Next(1, 5);
            var item = new Weapon(a);
            var tile = level.Map.RandomFloorTile();
            item.Location = tile.Location;
            PlaceEntity(item);
            item.EntityWasDestroyed += OnDestroyEntity;
        }   
    }

    public void UsePortal(Portal portal)
    {
        // TODO: this probably doesn't belong here, it should probably be a method on the portal
        if (portal.LinkedPortal != null)
        {
            CurrentLevelNumber = portal.LinkedPortal.DungeonLevel;
        }

        if (portal.LinkedPortal is null && portal.GetType() == typeof(StairsUp))
        {
            return;
        }
        if (portal.LinkedPortal is null && portal.GetType() == typeof(StairsDown))
        {
            AddDungeonLevel();
            CurrentLevelNumber++;
        }
        Player.Location = CurrentMap.RandomAdjacentTile(portal.LinkedPortal!.Location.To2D).Location;
        PlaceEntity(Player);
        EnemyManager.PopulateLevel(CurrentLevelNumber);
    }

    public DungeonLevel GetDungeonLevel(int level)
    {
        return _levels[level - 1];
    }

    public void PlaceEntity(Entity.Entity entity)
    {
        var l = (IDungeonLevel)GetDungeonLevel(entity.Location.Z);
        l.PlaceEntity(entity);
    }

    public void OnDestroyEntity(Entity.Entity sender, DestroyEventArgs args)
    {
        RemoveEntity(sender);
        foreach (var item in args.ItemsDropped)
        {
            item.Location = args.DestroyLocation;
            PlaceEntity(item);
            item.EntityWasDestroyed += OnDestroyEntity;
        }
    }

    public void RemoveEntity(Entity.Entity entity)
    {
        var l = (IDungeonLevel)GetDungeonLevel(entity.Location.Z);
        l.RemoveEntity(entity);
    }

    public void MoveEntity(Entity.Entity entity, IntVector3 loc)
    {
        RemoveEntity(entity);
        entity.Location = loc;
        PlaceEntity(entity);
    }

    public void MoveEntity(Entity.Entity entity, IntVector2 loc)
    {
        MoveEntity(entity, new IntVector3(loc, CurrentLevelNumber));
    }

    public void MoveEntity(Entity.Entity entity, int x, int y)
    {
        MoveEntity(entity, new IntVector3(x, y, CurrentLevelNumber));
    }
    
    public void MoveEntity(Entity.Entity entity, int x, int y, int z)
    {
        MoveEntity(entity, new IntVector3(x, y, z));
    }
}

public class InvalidDungeonLevelException : Exception
{
    
    public InvalidDungeonLevelException(string message) : base(message)
    {
        
    }
}
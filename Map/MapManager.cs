using System;
using System.Collections.Generic;
using Roguelike.Entity.Feature;

namespace Roguelike.Map;

public class MapManager : RoguelikeGameManager
{
    public TileMap CurrentMap => _maps[_dungeonLevelIndex];
    
    private int _dungeonLevelIndex = 0;
    public int CurrentDungeonLevel
    {
        get => _dungeonLevelIndex + 1;
        private set => _dungeonLevelIndex = value - 1;
    }

    private readonly List<TileMap> _maps = new();
    private Random _random = new ();

    public MapManager(RoguelikeGame game) : base(game)
    {
    }
    
    protected override void OnConnectManagers(object sender, EventArgs e)
    {
        // This event happens when all the manager classes are loaded. This is where we
        // subscribe to events from other managers.
        base.OnConnectManagers(sender, e);
    }

    public void InitializeMaps()
    {
        _maps.Clear();
        _dungeonLevelIndex = 0;
        AddDungeonLevel(1);
       
        Player.Location = _maps[^1].RandomAdjacentTile(_maps[^1].StairsUp.Location.To2D).Location;
        _maps[^1].Creatures[Player.Location.X, Player.Location.Y].Add(Player);
    }

    private void AddDungeonLevel(int level)
    {
        var generator = new MapGenerator();
        var map = generator.GenerateDungeonMap(level);
        _maps.Add(map);
        var newLevel = _maps.Count;
        map.StairsUp.DungeonLevel = newLevel;
        map.StairsDown.DungeonLevel = newLevel;
        if (newLevel > 1)
        {
            var prevLevel = newLevel - 1;
            _maps[prevLevel - 1].StairsDown.LinkedPortal = map.StairsUp;
            map.StairsUp.LinkedPortal = _maps[prevLevel - 1].StairsDown;
        }
    }

    public void MovePlayerToLevel(int level, IntVector2 spawnLoc)
    {
        if (level < 1 || level > _maps.Count)
        {
            throw new InvalidDungeonLevelException($"Tried to go to invalid dungeon level {level}");
        }

        var player = Game.Services.GetService<PlayerManager>().Player;
        player.Location = new IntVector3(spawnLoc.X, spawnLoc.Y, level);
    }

    // public void OnEnterTile(Entity.Entity entity, DungeonTile tile)
    // {
    //     if (entity is Player)
    //     {
    //         foreach (var f in CurrentMap.Features[tile.X, tile.Y])
    //         {
    //             if (f is Portal p)
    //             {
    //                 UsePortal(p);
    //                 return;
    //             }
    //         } 
    //     }
    // }

    public void UsePortal(Portal portal)
    {
        if (portal.LinkedPortal != null)
        {
            CurrentDungeonLevel = portal.LinkedPortal.DungeonLevel;
        }

        if (portal.LinkedPortal is null && portal.GetType() == typeof(StairsUp))
        {
            return;
            // throw new InvalidDungeonLevelException(
            // $"Tried to go upstairs on level {CurrentDungeonLevel}, but no stairs are linked");
        }
        if (portal.LinkedPortal is null && portal.GetType() == typeof(StairsDown))
        {
            AddDungeonLevel(portal.DungeonLevel + 1);
            // portal.LinkedPortal = _maps[^1].StairsUp;
            CurrentDungeonLevel++;
        }
        MovePlayerToLevel(CurrentDungeonLevel, CurrentMap.RandomAdjacentTile(portal.LinkedPortal!.Location.To2D).Location.To2D);
        EnemyManager.PopulateLevel(CurrentDungeonLevel);
    }

    // public void OnPlayerMoved(object sender, EventArgs args)
    // {
    //     var a = (MoveEventArgs)args;
    //     foreach (var f in CurrentMap.Features[a.ToLocation.X, a.ToLocation.Y])
    //     {
    //         if (f is Portal p)
    //         {
    //             UsePortal(p);
    //             return;
    //         }
    //     }
    // }
    
}

public class InvalidDungeonLevelException : Exception
{
    
    public InvalidDungeonLevelException(string message) : base(message)
    {
        
    }
}

public class LevelChangeEventArgs : EventArgs
{
    public int FromLevel;
    public int ToLevel;
    public Portal PortalUsed;
    public IntVector3 FromLocation;
    public IntVector3 ToLocation;

    public LevelChangeEventArgs(int fromLevel, int toLevel, Portal portalUsed)
    {
        FromLevel = fromLevel;
        ToLevel = toLevel;
        PortalUsed = portalUsed;
        FromLocation = portalUsed.Location;
        ToLocation = portalUsed.LinkedPortal.Location;
    }

    public LevelChangeEventArgs(int fromLevel, IntVector3 toLocation)
    {
        FromLevel = fromLevel;
        ToLevel = toLocation.Z;
        ToLocation = toLocation;
    }
}
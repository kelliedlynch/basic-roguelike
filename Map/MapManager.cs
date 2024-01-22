using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Roguelike.Entity;
using Roguelike.Entity.Feature;

namespace Roguelike.Map;

public class MapManager : GameComponent
{
    public TileMap CurrentMap => _maps[_dungeonLevelIndex];
    public int CurrentDungeonLevel
    {
        get => _dungeonLevelIndex + 1;
        set
        {
            _dungeonLevelIndex = value - 1;

        }
    }

    private int _dungeonLevelIndex = 0;
    private readonly List<TileMap> _maps = new();
    private Random _random = new ();

    // public event EventHandler NewLevelLoaded;
    
    public MapManager(RoguelikeGame game) : base(game)
    {
        game.BeginGame += BeginGame;
    }

    private void BeginGame(object sender, EventArgs e)
    {
      
        _maps.Clear();
        AddDungeonLevel();
        // NewLevelLoaded?.Invoke(this, EventArgs.Empty);
        
        var player = Game.Services.GetService<PlayerManager>().Player;
        player.Location = _maps[^1].RandomAdjacentTile(_maps[^1].StairsUp.Location, 2).Location;
        player.EntityMoved += OnPlayerMoved;
    }

    private void AddDungeonLevel()
    {
        var generator = new MapGenerator();
        var map = generator.GenerateDungeonMap();
        _maps.Add(map);
        var newLevel = _maps.Count;
        map.StairsUp.DungeonLevel = newLevel;
        map.StairsDown.DungeonLevel = newLevel;
        if (newLevel > 1)
        {
            var prevLevel = newLevel - 1;
            _maps[prevLevel - 1].StairsDown.LinkedPortal = map.StairsUp;
            map.StairsUp.LinkedPortal = _maps[prevLevel - 1].StairsDown;
            var eman = Game.Services.GetService<EnemyManager>();
            eman.Enemies.Add(new List<Creature>());
        }


    }

    public void MovePlayerToLevel(int level, IntVector2 spawnLoc)
    {
        if (level < 1 || level > _maps.Count)
        {
            throw new InvalidDungeonLevelException($"Tried to go to invalid dungeon level {level}");
        }
        // if (_currentDungeonLevel > _maps.Count)
        // {
        //     for (int i = 0; i <= _currentDungeonLevel - _maps.Count; i++)
        //     {
        //         var stairsDown = _maps[^2].StairsDown;
        //         AddDungeonLevel();
        //         _maps[^1].StairsUp.LinkedPortal = stairsDown;
        //         stairsDown.LinkedPortal = _maps[^1].StairsUp;
        //     }
        // }
        var player = Game.Services.GetService<PlayerManager>().Player;
        player.DungeonLevel = level;
        player.Location = spawnLoc;
        // NewLevelLoaded?.Invoke(this, EventArgs.Empty);
    }

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
            AddDungeonLevel();
            // portal.LinkedPortal = _maps[^1].StairsUp;
            CurrentDungeonLevel++;
        }
        MovePlayerToLevel(CurrentDungeonLevel, CurrentMap.RandomAdjacentTile(portal.LinkedPortal!.Location, 2).Location);
    }

    public void OnPlayerMoved(object sender, EventArgs args)
    {
        var a = (MoveEventArgs)args;
        foreach (var f in CurrentMap.Features[a.ToLocation.X, a.ToLocation.Y])
        {
            if (f is Portal p)
            {
                UsePortal(p);
                return;
            }
        }
    }
    
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
    public IntVector2 AtLocation;

    public LevelChangeEventArgs(int fromLevel, int toLevel, Portal portal)
    {
        FromLevel = fromLevel;
        ToLevel = toLevel;
        AtLocation = portal.Location;
    }

    public LevelChangeEventArgs(int fromLevel, int toLevel, IntVector2 atLocation)
    {
        FromLevel = fromLevel;
        ToLevel = toLevel;
        AtLocation = atLocation;
    }
}
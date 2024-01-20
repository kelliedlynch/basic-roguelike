using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Roguelike;

public class MapManager : GameComponent
{
    public TileMap CurrentMap => _maps[_currentDungeonLevel];
    public int CurrentDungeonLevel => _currentDungeonLevel + 1;
    
    private int _currentDungeonLevel = 0;
    private readonly List<TileMap> _maps = new();

    public event EventHandler NewLevelLoaded;
    
    public MapManager(RoguelikeGame game) : base(game)
    {
        game.BeginGame += BeginGame;
    }

    private void BeginGame(object sender, EventArgs e)
    {
        _maps.Clear();
        AddDungeonLevel();
        NewLevelLoaded?.Invoke(this, EventArgs.Empty);
    }

    private void AddDungeonLevel()
    {
        var generator = new MapGenerator();
        var map = generator.GenerateDungeonMap();
        _maps.Add(map);
    }

    public void IncreaseDungeonLevel(int levels = 1)
    {
        if (_currentDungeonLevel + levels >= _maps.Count)
        {
            for (int i = 0; i < levels; i++)
            {
                AddDungeonLevel();
            }
        }
        _currentDungeonLevel += levels;
        NewLevelLoaded?.Invoke(this, EventArgs.Empty);
    }

    public void DecreaseDungeonLevel(int levels = 1)
    {
        if (_currentDungeonLevel - levels < 0)
        {
            throw new InvalidDungeonLevelException($"Tried to go to invalid dungeon level {_currentDungeonLevel - levels}");
        }
    }
    
}

public class InvalidDungeonLevelException : Exception
{
    
    public InvalidDungeonLevelException(string message) : base(message)
    {
        
    }
}
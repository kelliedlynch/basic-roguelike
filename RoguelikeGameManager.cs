using System;
using Microsoft.Xna.Framework;
using Roguelike.Entity;
using Roguelike.Map;

namespace Roguelike;

public class RoguelikeGameManager : GameComponent
{
    
    public DrawEngine DrawEngine;
    public EnemyManager EnemyManager;
    public PlayerManager PlayerManager;
    // public MapManager MapManager;
    public InputManager InputManager;
    public TurnManager TurnManager;
    public ActivityLog ActivityLog;
    public LevelManager LevelManager;
    public Player Player => Game.Services.GetService<PlayerManager>().Player;
    
    public RoguelikeGameManager(RoguelikeGame game) : base(game)
    {
        game.ConnectManagers += OnConnectManagers;
    }

    protected virtual void OnConnectManagers(object sender, EventArgs e)
    {
        // This event happens when all the manager classes are loaded. This is where we
        // subscribe to events from other managers.
        DrawEngine = Game.Services.GetService<DrawEngine>();
        EnemyManager = Game.Services.GetService<EnemyManager>();
        PlayerManager = Game.Services.GetService<PlayerManager>();
        // MapManager = Game.Services.GetService<MapManager>();
        InputManager = Game.Services.GetService<InputManager>();
        TurnManager = Game.Services.GetService<TurnManager>();
        ActivityLog = Game.Services.GetService<ActivityLog>();
        LevelManager = Game.Services.GetService<LevelManager>();
    }
    
    protected virtual void OnBeginGame(object sender, EventArgs e)
    {
        // This event happens upon beginning a new game. Managers are loaded/triggered in this order:
        // Player -> Map -> Enemy -> Entity -> Input
    }



}
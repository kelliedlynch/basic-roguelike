using System;
using Microsoft.Xna.Framework;
using Roguelike.Entity;
using Roguelike.Graphics;
using Roguelike.Map;

namespace Roguelike;

public class RoguelikeGameManager : GameComponent
{
    
    public DrawEngine DrawEngine;
    public EnemyManager EnemyManager;
    public PlayerManager PlayerManager;
    public InputManager InputManager;
    public TurnManager TurnManager;
    public ActivityLog ActivityLog;
    public LevelManager LevelManager;
    public MenuManager MenuManager;
    public Player Player => Game.Services.GetService<PlayerManager>().Player;
    
    // public RoguelikeGameManager(RoguelikeGame game) : base(game)
    // {
    //     game.ConnectManagers += OnConnectManagers;
    // }
    
    public RoguelikeGameManager(Game game) : base(game)
    {
        var g = (RoguelikeGame)game;
        g.ConnectManagers += OnConnectManagers;
    }

    protected virtual void OnConnectManagers(object sender, EventArgs e)
    {
        // This event happens when all the manager classes are loaded. 
        DrawEngine = Game.Services.GetService<DrawEngine>();
        EnemyManager = Game.Services.GetService<EnemyManager>();
        PlayerManager = Game.Services.GetService<PlayerManager>();
        InputManager = Game.Services.GetService<InputManager>();
        TurnManager = Game.Services.GetService<TurnManager>();
        ActivityLog = Game.Services.GetService<ActivityLog>();
        LevelManager = Game.Services.GetService<LevelManager>();
        MenuManager = Game.Services.GetService<MenuManager>();
        
        AfterConnectManagers();
    }

    protected virtual void AfterConnectManagers()
    {
        
    }
    
    protected virtual void OnBeginGame(object sender, EventArgs e)
    {
        // This event happens upon beginning a new game. Managers are loaded/triggered in this order:
        // Player -> Map -> Enemy -> Entity -> Input
    }



}
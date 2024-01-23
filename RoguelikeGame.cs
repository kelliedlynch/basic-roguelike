using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Roguelike.Entity;
using Roguelike.Map;

namespace Roguelike;

public class RoguelikeGame : Game
{
    public GraphicsDeviceManager Graphics;
    public DrawEngine DrawEngine;
    public EnemyManager EnemyManager;
    public PlayerManager PlayerManager;
    public MapManager MapManager;
    public InputManager InputManager;
    public EntityManager EntityManager;
    public Player Player;

    public event EventHandler ConnectManagers;
    public event EventHandler BeginGame;


    // private void OnBeginGame(EventArgs e)
    // {
    //     var beginGame = BeginGame;
    //     beginGame?.Invoke(this, e);
    // }
    
    public RoguelikeGame()
    {
        Graphics = new GraphicsDeviceManager(this);
        Graphics.PreferredBackBufferHeight = 1120;
        Graphics.PreferredBackBufferWidth = 800;
        Content.RootDirectory = "Content";
        IsMouseVisible = true;

    }

    protected override void Initialize()
    {

        DrawEngine = new DrawEngine(this);
        Components.Add(DrawEngine);
        Services.AddService(typeof(DrawEngine), DrawEngine);
        
        PlayerManager = new PlayerManager(this);
        Components.Add(PlayerManager);
        Services.AddService(typeof(PlayerManager), PlayerManager);

        MapManager = new MapManager(this);
        Components.Add(MapManager);
        Services.AddService(typeof(MapManager), MapManager);
        
        EnemyManager = new EnemyManager(this);
        Components.Add(EnemyManager);
        Services.AddService(typeof(EnemyManager), EnemyManager);
        
        EntityManager = new EntityManager(this);
        Components.Add(EntityManager);
        Services.AddService(typeof(EntityManager), EntityManager);

        InputManager = new InputManager(this);
        Components.Add(InputManager);
        Services.AddService(typeof(InputManager), InputManager);

        ConnectManagers?.Invoke(this, EventArgs.Empty);

        base.Initialize();
    }

    protected override void LoadContent()
    {
        base.LoadContent();
    }

    protected override void BeginRun()
    {
        BeginGame?.Invoke(this, EventArgs.Empty);
        
        base.BeginRun();
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        if (Keyboard.GetState().IsKeyDown(Keys.Space))
        {
            BeginGame?.Invoke(this, EventArgs.Empty);
            // EnemyManager.Enemies = new List<Creature>();
            // PlayerManager.SpawnInPlayer(DrawEngine.TileMap);
        }

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black);
        base.Draw(gameTime);
    }
}

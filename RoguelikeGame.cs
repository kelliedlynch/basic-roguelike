using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Roguelike.Graphics;
using Roguelike.Map;

namespace Roguelike;

public class RoguelikeGame : Game
{
    public GraphicsDeviceManager Graphics;
    public DrawEngine DrawEngine;
    public EnemyManager EnemyManager;
    public PlayerManager PlayerManager;
    public InputManager InputManager;
    public TurnManager TurnManager;
    public ActivityLog ActivityLog;
    public LevelManager LevelManager;
    public SpriteBatch SpriteBatch;

    public event EventHandler ConnectManagers;
    public event EventHandler BeginNewGame;

    // public event EventHandler EndGame;
    
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

        // MapManager = new MapManager(this);
        // Components.Add(MapManager);
        // Services.AddService(typeof(MapManager), MapManager);
        
        EnemyManager = new EnemyManager(this);
        Components.Add(EnemyManager);
        Services.AddService(typeof(EnemyManager), EnemyManager);

        LevelManager = new LevelManager(this);
        Components.Add(LevelManager);
        Services.AddService(typeof(LevelManager), LevelManager);

        InputManager = new InputManager(this);
        Components.Add(InputManager);
        Services.AddService(typeof(InputManager), InputManager);

        TurnManager = new TurnManager(this);
        Components.Add(TurnManager);
        Services.AddService(TurnManager);

        ActivityLog = new ActivityLog(this);
        Components.Add(ActivityLog);
        Services.AddService(ActivityLog);

        ConnectManagers?.Invoke(this, EventArgs.Empty);
        
        SpriteBatch = new SpriteBatch(GraphicsDevice);
        Services.AddService(typeof(SpriteBatch), SpriteBatch);

        base.Initialize();
    }

    protected override void BeginRun()
    {
        BeginGame();
        // EndGame += OnEndGame;
        InputManager.GameState = GameRunningState.GameRunning;
        
        base.BeginRun();
    }

    public void BeginGame()
    {
        ActivityLog.InitializeLog();
        PlayerManager.InitializePlayer();
        LevelManager.InitializeLevels();
        EnemyManager.InitializeEnemies();
        PlayerManager.Player.EntityWasDestroyed += OnEndGame;
        InputManager.GameState = GameRunningState.GameRunning;
    }

    private void OnEndGame(object sender, EventArgs args)
    {
        PlayerManager.Player.EntityWasDestroyed -= OnEndGame;
        InputManager.GameState = GameRunningState.GameOver;
        
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        base.Update(gameTime);
    }

    protected override bool BeginDraw()
    {
        GraphicsDevice.Clear(Color.Black);
        SpriteBatch.Begin();
        return base.BeginDraw();
    }

    protected override void Draw(GameTime gameTime)
    {

        base.Draw(gameTime);
    }

    protected override void EndDraw()
    {
        SpriteBatch.End();
        base.EndDraw();
    }
}

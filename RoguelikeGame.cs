using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Roguelike.Entity;
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
    public MenuManager MenuManager;
    public SpriteBatch SpriteBatch;

    public event EventHandler ConnectManagers;
    
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
        SpriteBatch = new SpriteBatch(GraphicsDevice);
        Services.AddService(typeof(SpriteBatch), SpriteBatch);
        
        var font = Content.Load<SpriteFont>("Fonts/Kenney Mini");
        Services.AddService(font);

        DrawEngine = new DrawEngine(this);
        Components.Add(DrawEngine);
        Services.AddService(typeof(DrawEngine), DrawEngine);
        
        PlayerManager = new PlayerManager(this);
        AddManager(PlayerManager);
        
        EnemyManager = new EnemyManager(this);
        AddManager(EnemyManager);

        LevelManager = new LevelManager(this);
        AddManager(LevelManager);

        InputManager = new InputManager(this);
        AddManager(InputManager);

        TurnManager = new TurnManager(this);
        AddManager(TurnManager);

        ActivityLog = new ActivityLog(this);
        Components.Add(ActivityLog);
        Services.AddService(ActivityLog);

        MenuManager = new MenuManager(this);
        AddManager(MenuManager);
        

        ConnectManagers?.Invoke(this, EventArgs.Empty);
        
        base.Initialize();
    }

    private void AddManager(RoguelikeGameManager m)
    {
        Components.Add(m);
        Services.AddService(m.GetType(), m);
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

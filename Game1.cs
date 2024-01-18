using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Roguelike;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private Texture2D _testTexture;
    private TileEngine _tileEngine;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        _graphics.PreferredBackBufferHeight = 1120;
        _graphics.PreferredBackBufferWidth = 800;
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        
    }

    protected override void Initialize()
    {
        // TODO: Add your initialization logic here

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        // _level = new Level(Services, 50, 64);
        _tileEngine = new TileEngine(Services);
        var generator = new MapGenerator();
        _tileEngine.ChangeMap(generator);
        _tileEngine.SpawnInPlayer();
        // var pathFinder = new Pathfinder(_tileMap);
        // var path = pathFinder.FindPath(Vector2.Zero, new Vector2(5, 5));
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        if (Keyboard.GetState().IsKeyDown(Keys.Space))
        {
            var generator = new MapGenerator();
            _tileEngine.ChangeMap(generator);
        }

        _tileEngine.Update(gameTime);
        

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black);

        _spriteBatch.Begin();
        _tileEngine.Draw(gameTime, _spriteBatch);
        _spriteBatch.End();

        base.Draw(gameTime);
    }
}

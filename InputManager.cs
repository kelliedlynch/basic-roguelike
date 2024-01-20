using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Roguelike.Content.Entity;

namespace Roguelike;

public class InputManager : GameComponent
{
    private bool _keyIsPressed;
    private double _keyDownTime = 0;
    private const double FastMoveDelay = .4;
    private double _fastMoveTimer = 0;
    private const double FastMoveInterval = .15;

    public InputManager(RoguelikeGame game) : base(game)
    {
        
    }
    
    
    public override void Update(GameTime gameTime)
    {
        var manager = Game.Services.GetService<PlayerManager>();
        // var map = Game.Services.GetService<MapManager>().CurrentMap;
        var keyboard = Keyboard.GetState();
        if (!_keyIsPressed || (_keyIsPressed && _keyDownTime > FastMoveDelay) || _fastMoveTimer > FastMoveInterval)
        {
            if (keyboard.GetPressedKeys().Length == 0)
            {
                _keyIsPressed = false;
                return;
            }

            if (!_keyIsPressed)
            {
                _keyDownTime = 0;
                _keyIsPressed = true;
            }


            var destination = manager.Player.Location;
            if (keyboard.IsKeyDown(Keys.Up))
            {
                destination += Direction.Up;
            }
            else if (keyboard.IsKeyDown(Keys.Down))
            {
                destination += Direction.Down;
            }
            else if (keyboard.IsKeyDown(Keys.Left))
            {
                destination += Direction.Left;
            }
            else if (keyboard.IsKeyDown(Keys.Right))
            {
                destination += Direction.Right;
            }


            
            manager.AttemptMove(destination);
        }
        else if (_keyIsPressed && keyboard.GetPressedKeys().Length > 0)
        {
            _keyDownTime += gameTime.ElapsedGameTime.TotalSeconds;
            if (_keyDownTime > FastMoveDelay)
            {
                _fastMoveTimer += gameTime.ElapsedGameTime.TotalSeconds;
            }
        }
        else
        {
            _keyIsPressed = false;
            _keyDownTime = 0;
            _fastMoveTimer = 0;
        }

        
    }
}

public struct Direction
{
    public static IntVector2 Up { get; } = new(0, -1);
    public static IntVector2 Down { get; } = new(0, 1);
    public static IntVector2 Left { get; } = new(-1, 0);
    public static IntVector2 Right { get; } = new(1, 0);
}


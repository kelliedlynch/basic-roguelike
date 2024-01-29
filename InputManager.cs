using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using StateMachine;

namespace Roguelike;

public class InputManager : RoguelikeGameManager
{
    public InputState GameState = InputState.GameOver;
    private bool _keyIsPressed;
    private double _keyDownTime;
    private const double FastMoveDelay = .4;
    private double _fastMoveTimer;
    private const double FastMoveInterval = .15;

    public event EventHandler BeginNewGame;

    
    private enum TurnState {Waiting, Processing}
    private enum InputTrigger {ArrowPressed}
    private Fsm<TurnState, InputTrigger> inputMachine;



    public InputManager(RoguelikeGame game) : base(game)
    {

    }

    public override void Update(GameTime gameTime)
    {

        var keyboard = Keyboard.GetState();
        if (GameState == InputState.GameOver)
        {
            if (keyboard.IsKeyDown(Keys.Space))
            {
                BeginNewGame?.Invoke(this, EventArgs.Empty);
            }

            return;
        }
        
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


            var destination = Player.Location.To2D;
            if (keyboard.IsKeyDown(Keys.Up) || keyboard.IsKeyDown(Keys.NumPad8))
            {
                destination += Direction.Up;
            }
            else if (keyboard.IsKeyDown(Keys.Down) || keyboard.IsKeyDown(Keys.NumPad2))
            {
                destination += Direction.Down;
            }
            else if (keyboard.IsKeyDown(Keys.Left) || keyboard.IsKeyDown(Keys.NumPad4))
            {
                destination += Direction.Left;
            }
            else if (keyboard.IsKeyDown(Keys.Right) || keyboard.IsKeyDown(Keys.NumPad6))
            {
                destination += Direction.Right;
            }
            else if (keyboard.IsKeyDown(Keys.NumPad7))
            {
                destination += Direction.Left + Direction.Up;
            }
            else if (keyboard.IsKeyDown(Keys.NumPad9))
            {
                destination += Direction.Right + Direction.Up;
            }
            else if (keyboard.IsKeyDown(Keys.NumPad1))
            {
                destination += Direction.Left + Direction.Down;
            }
            else if (keyboard.IsKeyDown(Keys.NumPad3))
            {
                destination += Direction.Right + Direction.Down;
            }
            else if (keyboard.IsKeyDown(Keys.NumPad5))
            {
                TurnManager.ProcessTurn();
                return;
            }
            Console.WriteLine("attempting move");
            PlayerManager.AttemptMove(new IntVector3(destination, Player.Location.Z));
            Console.WriteLine("move attempted");
            TurnManager.ProcessTurn();
            
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

public enum InputState
{
    GameRunning,
    GameOver
}
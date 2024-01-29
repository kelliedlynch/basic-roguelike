using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Roguelike.Event;

namespace Roguelike;

public class InputManager : RoguelikeGameManager
{
    public GameRunningState GameState = GameRunningState.GameOver;
    private const double FastMoveDelay = .4;
    private const double StandardMoveInterval = .1;
    private const double FastMoveInterval = .01;
    
    private double _keyDownTime;
    private bool _fastMoveEngaged;
    private double MoveInterval => _fastMoveEngaged ? FastMoveInterval : StandardMoveInterval;


    private double _moveTimer;
    

    private InputState _inputState = InputState.Ready;
    
    private Keys[] _lastKeysPressed = Array.Empty<Keys>();

    public event KeyEventHandler KeyEvent;
    public event MoveEventHandler AttemptedMove;

    private enum InputState {Ready, Processing}

    private static readonly List<Keys> MovementKeys = new()
    {
        Keys.Up, Keys.Down, Keys.Left, Keys.Right, Keys.NumPad1, Keys.NumPad2, Keys.NumPad3, Keys.NumPad4, Keys.NumPad5,
        Keys.NumPad6, Keys.NumPad7, Keys.NumPad8, Keys.NumPad9
    };


    public InputManager(RoguelikeGame game) : base(game)
    {
        KeyEvent += OnKeyEvent;
    }

    protected override void OnBeginGame(object sender, EventArgs e)
    {
        _keyDownTime = 0;
        _fastMoveEngaged = false;
        _moveTimer = 0;
        _lastKeysPressed = Array.Empty<Keys>();
        _inputState = InputState.Ready;
        base.OnBeginGame(sender, e);
    }

    protected override void AfterConnectManagers()
    { 
        TurnManager.TurnCompleted += OnTurnCompleted;
        base.AfterConnectManagers();
    }

    private void OnTurnCompleted(object sender, EventArgs args)
    {
        _moveTimer = 0;
        _inputState = InputState.Ready;
    }

    private void OnKeyEvent(KeyEventArgs args)
    {
        if (MovementKeys.Contains(args.Key))
        {
            if (args.State == KeyState.Down)
            {
                _inputState = InputState.Processing;
                AttemptedMove?.Invoke(BuildMoveEvent(args.Key));
            }
            else if (args.State == KeyState.Held)
            {
                _keyDownTime += args.Time;
                if (_fastMoveEngaged)
                {
                    _moveTimer += args.Time;
                    if (_moveTimer > MoveInterval)
                    {
                        _inputState = InputState.Processing;
                        AttemptedMove?.Invoke(BuildMoveEvent(args.Key));
                    }
                }
                else
                {
                    if (_keyDownTime > FastMoveDelay)
                    {
                        _inputState = InputState.Processing;
                        _fastMoveEngaged = true;
                        AttemptedMove?.Invoke(BuildMoveEvent(args.Key));
                    }
                }
                

            }

            else if (args.State == KeyState.Up)
            {
                _fastMoveEngaged = false;
                _keyDownTime = 0;
                _moveTimer = 0;
                _lastKeysPressed = Array.Empty<Keys>();
            }
        }
    }

    private MoveEventArgs BuildMoveEvent(Keys key)
    {
        var args = new MoveEventArgs(Player);
        var destination = Player.Location;
        switch (key)
        {
            case Keys.Up or Keys.NumPad8:
                destination += Direction.Up;
                break;
            case Keys.Down or Keys.NumPad2:
                destination += Direction.Down;
                break;
            case Keys.Left or Keys.NumPad4:
                destination += Direction.Left;
                break;
            case Keys.Right or Keys.NumPad6:
                destination += Direction.Right;
                break;
            case Keys.NumPad7:
                destination += Direction.Left + Direction.Up;
                break;
            case Keys.NumPad9:
                destination += Direction.Right + Direction.Up;
                break;
            case Keys.NumPad1:
                destination += Direction.Left + Direction.Down;
                break;
            case Keys.NumPad3:
                destination += Direction.Right + Direction.Down;
                break;
            case Keys.NumPad5:
                break;
        }

        args.ToLocation = destination;
        return args;
    }
    
    public override void Update(GameTime gameTime)
    {
        
        
        // if (_inputState != InputState.Ready) return;
        
        var keyboard = Keyboard.GetState();
        var pressed = keyboard.GetPressedKeys();
        // if(pressed.Length > 0) Console.WriteLine("update");

        if (pressed.Contains(Keys.Space))
        {
            var g = (RoguelikeGame)Game;
            g.BeginGame();
            return;
        }

        if (_inputState == InputState.Ready)
        {
            foreach (var key in _lastKeysPressed)
            {
                var args = new KeyEventArgs()
                {
                    Key = key
                };

                if (!pressed.Contains(key))
                {

                    args.State = KeyState.Up;
                    KeyEvent?.Invoke(args);
                }
                else
                {
                    // else key is held down
                    args.State = KeyState.Held;
                    args.Time = gameTime.ElapsedGameTime.TotalSeconds;
                    KeyEvent?.Invoke(args);
                }
            }

            foreach (var key in pressed)
            {
                if (_lastKeysPressed.Contains(key)) continue;
                var args = new KeyEventArgs()
                {
                    Key = key,
                    State = KeyState.Down
                };
                KeyEvent?.Invoke(args);
            }
        }

        _lastKeysPressed = pressed;
        
    }
}

public struct Direction
{
    public static IntVector3 Up { get; } = new(0, -1, 0);
    public static IntVector3 Down { get; } = new(0, 1, 0);
    public static IntVector3 Left { get; } = new(-1, 0, 0);
    public static IntVector3 Right { get; } = new(1, 0, 0);
    public static IntVector3 LevelUp { get; } = new(0, 0, -1);
    public static IntVector3 LevelDown { get; } = new(0, 0, 1);
}

public enum GameRunningState
{
    GameRunning,
    GameOver
}

public delegate void KeyEventHandler(KeyEventArgs args);
public enum KeyState{ Up, Down, Held}

public class KeyEventArgs : EventArgs
{
    public Keys Key;
    public KeyState State;
    public double Time;
}
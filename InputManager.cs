using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using StateMachine;
using StateMachine.Fluent.Api;

namespace Roguelike;

public class InputManager : GameComponent
{
    private bool _keyIsPressed;
    private double _keyDownTime;
    private const double FastMoveDelay = .4;
    private double _fastMoveTimer;
    private const double FastMoveInterval = .15;

    // private enum PlayerState
    // {
    //     MOVING,
    //     WAITING
    // };
    //
    // private enum PlayerTrigger
    // {
    //     ARROW_PRESSED,
    //     MOVE_COMPLETED
    // }
    // private Fsm<PlayerState, PlayerTrigger> _playerMachine;

    public InputManager(RoguelikeGame game) : base(game)
    {
        // _playerMachine = Fsm<PlayerState, PlayerTrigger>.Builder(PlayerState.WAITING)
        //     .State(PlayerState.WAITING)
        //         .TransitionTo(PlayerState.MOVING).On(PlayerTrigger.ARROW_PRESSED)
        //         .OnEnter(e =>
        //         {
        //
        //         })
        //     .State(PlayerState.MOVING)
        //         .TransitionTo(PlayerState.WAITING).On(PlayerTrigger.MOVE_COMPLETED)
        //         .OnEnter(e =>
        //         {
        //             
        //         })
        //     .Build();
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


            var destination = Game.Services.GetService<PlayerManager>().Player.Location.To2D;
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

            
            
            manager.AttemptMove(new IntVector3(destination, manager.Player.Location.Z));
            
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


using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Roguelike.Entity;
using Roguelike.Entity.Creature;
using Roguelike.Entity.Feature;
using Roguelike.Utility;

namespace Roguelike;

public class TurnManager : RoguelikeGameManager
{
    // private bool _isProcessing = false;

    private TurnPhase _phase = TurnPhase.Wait;

    // public event EventHandler EnterMovePhase;

    private Queue<MoveEventArgs> _moveQueue = new();
    private Queue<AttackEventArgs> _attackQueue = new();

    public TurnManager(RoguelikeGame game) : base(game)
    {
        
    }

    public void QueueMove(MoveEventArgs move)
    {
        _moveQueue.Enqueue(move);
    }

    public void QueueAttack(AttackEventArgs attack)
    {
        _attackQueue.Enqueue(attack);
    }
    

    public void ProcessTurn()
    {
        if (_phase == TurnPhase.Wait)
        {
            _phase = TurnPhase.PlayerMove;
        }
    }

    public override void Update(GameTime gameTime)
    {
        switch (_phase)
        {
            case TurnPhase.PlayerMove:
            {
                // TODO: A queue is probably not needed here.
                
                while (_moveQueue.Count > 0)
                {
                    var move = _moveQueue.Dequeue();
                    LevelManager.MoveEntity(Player, move.ToLocation);
                }
          
                _phase = _phase.Next();
                break;
            }
            case TurnPhase.PlayerAttack:
            {
                // TODO: Again, we probably don't need a queue for player actions
                while (_attackQueue.Count > 0)
                {
                    var attack = _attackQueue.Dequeue();
                    attack.Attacker.AttackEntity(attack.Defender);
                }

                _phase = _phase.Next();
                break;
            }
            case TurnPhase.EnemyMove:
            {
                EnemyManager.ProcessEnemyMoves();

                _phase = _phase.Next();
                break;
            }
            case TurnPhase.EnemyAttack:
            {
                EnemyManager.ProcessEnemyAttacks();

                _phase = _phase.Next();
                break;
            }
            case TurnPhase.PreAction:
            {
                var pickups = LevelManager.CurrentLevel.MoneyAt(Player.Location.X, Player.Location.Y);
                for (var index = pickups.Count - 1; index >= 0; index--)
                {
                    var entity = pickups[index];
                    if (entity.Location == Player.Location)
                    {
                        Player.PickUp(entity);
                    }
                }
                _phase = _phase.Next();
                break;
            }
            case TurnPhase.Action:
            {
                var portals = LevelManager.CurrentLevel.PortalsAt(Player.Location.X, Player.Location.Y);
                foreach (var portal in portals)
                {
                    LevelManager.UsePortal(portal);
                    break;
                }
                _phase = _phase.Next();
                break;
            }
            case TurnPhase.Spawn:
            {
                EnemyManager.RunSpawnCycle();
                _phase = _phase.Next();
                break;
            }
            case TurnPhase.EndTurn:
            {
                EnemyManager.EndTurn();
                _phase = _phase.Next();
                break;
            }
        }
        

        base.Update(gameTime);
    }
}

public class AttackEventArgs : EventArgs
{
    public Creature Attacker;
    public Creature Defender;

    public AttackEventArgs(Creature atk, Creature def)
    {
        Attacker = atk;
        Defender = def;
    }
}

public enum TurnPhase
{
    Wait,
    PlayerMove,
    PlayerAttack,
    EnemyMove,
    EnemyAttack,
    PreAction,
    Action,
    Spawn,
    EndTurn
}
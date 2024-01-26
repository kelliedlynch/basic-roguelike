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
    

    public void ProcessTurn(object sender, EventArgs args)
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
                while (_moveQueue.Count > 0)
                {
                    var move = _moveQueue.Dequeue();
                    move.Entity.Location = move.ToLocation;
                }
          
                _phase = _phase.Next();
                break;
            }
            case TurnPhase.Attack:
            {
                // currently we need to loop twice because if an enemy is destroyed, its attack is not removed from the queue
                // TODO: change that
                while (_attackQueue.Count > 0)
                {
                    var attack = _attackQueue.Dequeue();
                    attack.Attacker.AttackEntity(attack.Defender);
                }
                EnemyManager.QueueEnemyAttacks();
                while (_attackQueue.Count > 0)
                {
                    var attack = _attackQueue.Dequeue();
                    attack.Attacker.AttackEntity(attack.Defender);
                }
                _phase = _phase.Next();
                break;
            }
            case TurnPhase.PreAction:
            {
                var entities = EntityManager.EntitiesOnLevel(MapManager.CurrentDungeonLevel);
                for (var index = entities.Count - 1; index >= 0; index--)
                {
                    var entity = entities[index];
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
                var features = MapManager.CurrentMap.Features[Player.Location.X, Player.Location.Y];
                foreach (var feature in features)
                {
                    if (feature is Portal portal)
                    {
                        MapManager.UsePortal(portal);
                        break;
                    }
                }
                _phase = _phase.Next();
                break;
            }
            case TurnPhase.EnemyMove:
            {
                EnemyManager.QueueEnemyMoves();
                while (_moveQueue.Count > 0)
                {
                    var move = _moveQueue.Dequeue();
                    move.Entity.Location = move.ToLocation;
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
    Attack,
    PreAction,
    Action,
    EnemyMove,
    Spawn
}
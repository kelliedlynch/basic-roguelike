using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Roguelike.Entity;
using Roguelike.Entity.Creature;
using Roguelike.Entity.Feature;
using Roguelike.Map;
using Roguelike.Utility;
using StateMachine;

namespace Roguelike;

public class TurnManager : RoguelikeGameManager
{
    // private bool _isProcessing = false;

    private TurnPhase _phase = TurnPhase.WAIT;

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
        if (_phase == TurnPhase.WAIT)
        {
            _phase = TurnPhase.MOVE;
        }
    }

    public override void Update(GameTime gameTime)
    {
        switch (_phase)
        {
            case TurnPhase.MOVE:
            {
                while (_moveQueue.Count > 0)
                {
                    var move = _moveQueue.Dequeue();
                    move.Entity.Location = move.ToLocation;
                }
                break;
            }
            case TurnPhase.ATTACK:
            {
                while (_attackQueue.Count > 0)
                {
                    var attack = _attackQueue.Dequeue();
                    attack.Attacker.AttackEntity(attack.Defender);
                }
                break;
            }
            case TurnPhase.PRE_ACTION:
            {
                var mapman = Game.Services.GetService<MapManager>();
                var entities = Game.Services.GetService<EntityManager>().EntitiesOnLevel(mapman.CurrentDungeonLevel);
                var player = Game.Services.GetService<PlayerManager>().Player;
                for (var index = entities.Count - 1; index >= 0; index--)
                {
                    var entity = entities[index];
                    if (entity.Location == player.Location)
                    {
                        player.PickUp(entity);
                    }
                }
                break;
            }
            case TurnPhase.ACTION:
            {
                var player = Game.Services.GetService<PlayerManager>().Player;
                var mapManager = Game.Services.GetService<MapManager>();
                var features = mapManager.CurrentMap.Features[player.Location.X, player.Location.Y];
                foreach (var feature in features)
                {
                    if (feature is Portal portal)
                    {
                        mapManager.UsePortal(portal);
                        break;
                    }
                }
                break;
            }
            case TurnPhase.SPAWN:
            {
                var eman = Game.Services.GetService<EnemyManager>();
                eman.RunSpawnCycle();
                break;
            }
        }
        _phase = _phase.Next();

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
    WAIT,
    MOVE,
    ATTACK,
    PRE_ACTION,
    ACTION,
    SPAWN
}
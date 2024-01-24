using System;
using System.Buffers.Text;
using Microsoft.Xna.Framework;
using Roguelike.Entity;
using Roguelike.Map;

namespace Roguelike;

public class PlayerManager : RoguelikeGameManager
{
    public Player Player = new();
  
    public PlayerManager(RoguelikeGame game) : base(game)
    {
    }

    protected override void OnConnectManagers(object sender, EventArgs e)
    {
        // This event happens when all the manager classes are loaded. This is where we
        // subscribe to events from other managers.
        base.OnConnectManagers(sender, e);
    }
    
    protected override void OnBeginGame(object sender, EventArgs e)
    {
        // This event happens upon beginning a new game. Managers are loaded/triggered in this order:
        // Player -> Map -> Enemy -> Entity -> Input
        base.OnBeginGame(sender, e);
    }

    public void AttemptMove(IntVector3 loc)
    {
        var mapManager = Game.Services.GetService<MapManager>();
        var map = mapManager.CurrentMap;
        var turnManager = Game.Services.GetService<TurnManager>();
        if (!Player.CanMoveToTile(map, map.GetTileAt(loc.To2D))) return;
        
        var enemies = Game.Services.GetService<EnemyManager>().EnemiesOnLevel(mapManager.CurrentDungeonLevel);
        var skipMove = false;
        for (var index = enemies.Count - 1; index >= 0; index--)
        {
            var enemy = enemies[index];
            if (enemy.Location != loc) continue;
            var atk = new AttackEventArgs(Player, enemy);
            turnManager.QueueAttack(atk);
            skipMove = true;
        }
        
        if(!skipMove)
        {
            var move = new MoveEventArgs(Player, Player.Location, loc);

            turnManager.QueueMove(move);
        }
        
        // TODO: SHOULD THIS BE CALLED SOMEWHERE ELSE?
        turnManager.ProcessTurn(this, EventArgs.Empty);
    }
}
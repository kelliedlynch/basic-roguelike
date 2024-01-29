using System;
using Roguelike.Entity;
using Roguelike.Event;

namespace Roguelike;

public class PlayerManager : RoguelikeGameManager
{
    public new Player Player = new();
  
    public PlayerManager(RoguelikeGame game) : base(game)
    {
    }
    
    public void InitializePlayer()
    {
        Player = new();
        Player.OnLogEvent += ActivityLog.LogEvent;
    }

    public void AttemptMove(IntVector3 loc)
    {
        if (!Player.CanMoveToTile(LevelManager.CurrentLevel, LevelManager.CurrentMap.GetTileAt(loc.To2D))) return;

        var enemies = LevelManager.CurrentLevel.EnemiesOnLevel();
        var skipMove = false;
        for (var index = enemies.Count - 1; index >= 0; index--)
        {
            var enemy = enemies[index];
            if (enemy.Location != loc) continue;
            var atk = new AttackEventArgs(Player, enemy);
            TurnManager.QueueAttack(atk);
            skipMove = true;
        }
        
        if(!skipMove)
        {
            var move = new MoveEventArgs(Player, Player.Location, loc);

            TurnManager.QueueMove(move);
        }
    }
}
using System;
using System.Buffers.Text;
using Microsoft.Xna.Framework;
using Roguelike.Entity;
using Roguelike.Map;

namespace Roguelike;

public class PlayerManager : RoguelikeGameManager
{
    public Player Player = new();

    public event EventHandler AdvanceTurn;
    
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

    // public void SpawnInPlayer(object sender, EventArgs e)
    // {
    //     var map = Game.Services.GetService<MapManager>().CurrentMap;
    //     Player.Location = map.EntryPoint;
    // }

    public void AttemptMove(IntVector3 loc)
    {
        var man = Game.Services.GetService<MapManager>();
        var map = man.CurrentMap;
        var path = Player.Pathfinder.FindPath(map, Player.Location.To2D, loc.To2D);
        if (path is null || path.Count == 0)
        {
            return;
        }
        var enemies = Game.Services.GetService<EnemyManager>().EnemiesOnLevel(man.CurrentDungeonLevel);
        for (var index = enemies.Count - 1; index >= 0; index--)
        {
            var enemy = enemies[index];
            if (enemy.Location == loc)
            {
                Player.AttackEntity(enemy);
                return;
            }
        }

        var entities = Game.Services.GetService<EntityManager>().Entities;
        for (var index = entities.Count - 1; index >= 0; index--)
        {
            var entity = entities[index];
            if (entity.Location == loc)
            {
                Player.PickUp(entity);
            }
        }

        Player.Location = loc;
        AdvanceTurn?.Invoke(this, EventArgs.Empty);
        
    }
}
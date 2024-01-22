using System;
using Microsoft.Xna.Framework;
using Roguelike.Entity;
using Roguelike.Map;

namespace Roguelike;

public class PlayerManager : GameComponent
{
    public Player Player = new();
    
    public PlayerManager(RoguelikeGame game) : base(game)
    {
    }

    public override void Initialize()
    {
        // Game.Services.GetService<MapManager>().NewLevelLoaded += SpawnInPlayer;

        base.Initialize();
    }

    // public void SpawnInPlayer(object sender, EventArgs e)
    // {
    //     var map = Game.Services.GetService<MapManager>().CurrentMap;
    //     Player.Location = map.EntryPoint;
    // }

    public void AttemptMove(IntVector2 loc)
    {
        var man = Game.Services.GetService<MapManager>();
        var map = man.CurrentMap;
        var path = Player.Pathfinder.FindPath(map, Player.Location, loc);
        if (path is null || path.Count == 0)
        {
            return;
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

        var enemies = Game.Services.GetService<EnemyManager>().Enemies[man.CurrentDungeonLevel - 1];
        for (var index = enemies.Count - 1; index >= 0; index--)
        {
            var enemy = enemies[index];
            if (enemy.Location == loc)
            {
                Player.AttackEntity(enemy);
                return;
            }
        }

        Player.Location = loc;
    }
}
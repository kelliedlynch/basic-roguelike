using System;
using Microsoft.Xna.Framework;

namespace Roguelike;

public class PlayerManager : GameComponent
{
    public Player Player = new();
    
    public PlayerManager(RoguelikeGame game) : base(game)
    {
    }

    public override void Initialize()
    {
        Game.Services.GetService<MapManager>().NewLevelLoaded += SpawnInPlayer;

        base.Initialize();
    }

    public void SpawnInPlayer(object sender, EventArgs e)
    {
        var map = Game.Services.GetService<MapManager>().CurrentMap;
        Player.Location = map.EntryPoint;
    }

    public void AttemptMove(IntVector2 loc)
    {
        var map = Game.Services.GetService<MapManager>().CurrentMap;
        var path = Player.Pathfinder.FindPath(map, Player.Location, loc);
        if (path is null || path.Count == 0)
        {
            return;
        }

        var enemies = Game.Services.GetService<EnemyManager>().Enemies;
        foreach (var enemy in enemies)
        {
            if (enemy.Location == loc)
            {
                Player.AttackEntity(enemy);
                return;
            } 
        }

        Player.Location = loc;
    }
}
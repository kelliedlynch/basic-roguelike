using System;
using Microsoft.Xna.Framework;

namespace Roguelike;

public class PlayerManager : GameComponent
{
    public Player Player;
    
    public PlayerManager(RoguelikeGame game) : base(game)
    {
        game.BeginGame += CreateNewPlayer;
    }

    public override void Initialize()
    {

        base.Initialize();
    }

    public void CreateNewPlayer(object sender, EventArgs e)
    {
        var map = Game.Services.GetService<DrawEngine>().TileMap;
        Player = new Player();
        SpawnInPlayer(map);
    }

    public void SpawnInPlayer(TileMap map)
    {
        Player.Location = map.EntryPoint;
    }
}
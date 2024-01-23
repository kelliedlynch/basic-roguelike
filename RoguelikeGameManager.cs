using System;
using Microsoft.Xna.Framework;

namespace Roguelike;

public class RoguelikeGameManager : GameComponent
{
    public RoguelikeGameManager(RoguelikeGame game) : base(game)
    {
        game.ConnectManagers += OnConnectManagers;
        game.BeginGame += OnBeginGame;
    }

    protected virtual void OnConnectManagers(object sender, EventArgs e)
    {
        // This event happens when all the manager classes are loaded. This is where we
        // subscribe to events from other managers.
    }
    
    protected virtual void OnBeginGame(object sender, EventArgs e)
    {
        // This event happens upon beginning a new game. Managers are loaded/triggered in this order:
        // Player -> Map -> Enemy -> Entity -> Input
    }



}
using Microsoft.Xna.Framework;
using Roguelike.Graphics.UserInterface;

namespace Roguelike;

public class MenuManager : RoguelikeGameManager
{
    public InventoryMenu InventoryMenu;
    
    public MenuManager(Game game) : base(game)
    {
        InventoryMenu = new InventoryMenu(game);
        Game.Components.Add(InventoryMenu);
    }
}
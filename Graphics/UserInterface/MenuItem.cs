using Microsoft.Xna.Framework;
using Roguelike.Graphics.Layout;

namespace Roguelike.Graphics.UserInterface;

public class MenuItem : TextLabel
{
    public MenuItem(Game game) : base(game)
    {
    }

    public MenuItem(Game game, Rectangle bounds) : base(game, bounds)
    {
    }

    public MenuItem(Game game, Rectangle bounds, string text) : base(game, bounds, text)
    {
    }

    public MenuItem(Game game, string text) : base(game, text)
    {
    }
}
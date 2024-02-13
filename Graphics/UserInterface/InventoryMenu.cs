using System;
using Microsoft.Xna.Framework;
using Roguelike.Graphics.Layout;

namespace Roguelike.Graphics.UserInterface;

public class InventoryMenu : SpritePanel
{
    public VStackContainer ListContainer;
    
    public InventoryMenu(Game game) : base(game)
    {
        Visible = false;
        ListContainer = new VStackContainer(Game);
        ListContainer.Sizing = AxisSizing.ExpandXExpandY;
        AddChild(ListContainer);
        // VisibleChanged += OnVisibleChanged;
    }

    protected override void OnVisibleChanged(object sender, EventArgs args)
    {
        if (ListContainer is not null)
        {
            foreach (var child in ListContainer.Children)
            {
                child.Visible = Visible;
            }
        }

        base.OnVisibleChanged(sender, args);
    }

    public override void LayoutElements()
    {
        var player = Game.Services.GetService<PlayerManager>().Player;
        ListContainer.RemoveChildren();
        foreach (var item in player.Inventory.AllItems)
        {
            var label = new TextLabel(Game, item.Name);
            if (item == player.Inventory.Weapon)
            {
                label.Text = "* " + item.Name;
            }
            ListContainer.AddChild(label);
            label.Debug = true;
        }

        foreach (var child in Children)
        {
            child.LayoutElements();
        }
    }

    // protected override void OnVisibleChanged(object sender, EventArgs args)
    // {
    //     if (Visible)
    //     {
    //         BuildLayout();
    //     }
    //     base.OnVisibleChanged(sender, args);
    // }

    // public override void Draw(GameTime gameTime)
    // {
    //     BuildLayout();
    //     base.Draw(gameTime);
    // }
}
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Roguelike.UserInterface;

namespace Roguelike.Graphics.Layout;

public class Container : DrawnElement
{
    public Alignment ContentAlignment = Alignment.Center;

    public IntVector2 ContentsSize = IntVector2.Zero;





    public Container(Game game) : base(game)
    {
        Bounds = Rectangle.Empty;
    }
    
    public Container(Game game, Rectangle bounds) : base(game)
    {
        Bounds = bounds;
    }



    public virtual void LayoutElements()
    {
        foreach (var child in Children)
        {
            child.Position = Position + child.LocalPosition;
        }
    }

    public override void Draw(GameTime gameTime)
    {
        LayoutElements();
        base.Draw(gameTime);
    }

    public override void AddToBatch(SpriteBatch batch)
    {
        foreach (var element in Children)
        {
            element.AddToBatch(batch);            
        };
    }
}
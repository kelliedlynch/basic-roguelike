using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Roguelike.Graphics.Layout;
using Roguelike.UserInterface;

namespace Roguelike.Graphics;

public class DrawnElement : DrawableGameComponent
{
    public DrawnElement Parent;
    public RectangleShape Outline;
    public List<DrawnElement> Children = new();

    public IntVector2 Position
    {
        get
        {
            var p = Parent?.Position ?? IntVector2.Zero;
            return LocalPosition + p;
        }
        set
        {
            var p = Parent?.Position ?? IntVector2.Zero;
            LocalPosition = value - p;
        }
    }
    
    public IntVector2 LocalPosition;

    // public IntVector2 Size
    // {
    //     get
    //     {
    //         if (Sizing == SizingMode.ExpandToFit)
    //         {
    //             if (Parent is not null)
    //             {
    //                 return Parent.Size;
    //             }
    //
    //             var screen = GraphicsDevice.Viewport.Bounds.Size;
    //             return new IntVector2(screen.X, screen.Y);
    //         }
    //
    //         return _assignedSize;
    //     }
    //     set
    //     {
    //         _assignedSize = value;
    //     }
    // }

    // public SizingMode Sizing = SizingMode.FitContent;
    public AxisSizing Sizing = AxisSizing.ShrinkXShrinkY;

    public IntVector2 Size = IntVector2.Zero;
    public IntVector2 MinSize = IntVector2.Zero;
    public IntVector2 LayoutSize = IntVector2.Zero;

    public Rectangle Bounds
    {
        get
        {
            return new (Position, Size);
        }
        set
        {
            var oldSize = Size;
            Position = new IntVector2(value.X, value.Y);
            Size = new IntVector2(value.Width, value.Height);
            Outline.Bounds = value;
            if (oldSize.X != Size.X)
            {
                ChangedSizeX?.Invoke(this);
            }

            if (oldSize.Y != Size.Y)
            {
                ChangedSizeY?.Invoke(this);
            }
        }
    }

    public delegate void SizeChangeEventHandler(DrawnElement sender);
    public event SizeChangeEventHandler ChangedSizeX;
    public event SizeChangeEventHandler ChangedSizeY;
    
    public DrawnElement(Game game) : base(game)
    {
        Outline = new RectangleShape(Game);
        Outline.DrawOrder = DrawOrder + 1;
        Game.Components.Add(Outline);
    }
    
    public virtual void AddChild(DrawnElement e)
    {
        e.Parent = this;
        ChangedSizeX += e.OnChangedSizeX;
        ChangedSizeY += e.OnChangedSizeY;
        
        Children.Add(e);
        Game.Components.Add(e);
    }

    protected virtual void OnChangedSizeX(DrawnElement e)
    {
        if ((Sizing & (AxisSizing.ExpandX | AxisSizing.ShrinkX)) != 0)
        {
            Size = new IntVector2(e.Size.X, Size.Y);
        }
    }
    
    protected virtual void OnChangedSizeY(DrawnElement e)
    {
        if ((Sizing & (AxisSizing.ExpandY | AxisSizing.ShrinkY)) != 0)
        {
            Size = new IntVector2(Size.X, e.Size.Y);
        }
    }

    protected override void LoadContent()
    {
        // Outline = new RectangleShape(Game);
        // Game.Components.Add(Outline);
        base.LoadContent();
    }

    public virtual void AddToBatch(SpriteBatch batch)
    {

    }

    public override void Draw(GameTime gameTime)
    {
        Outline.Bounds = Bounds;
        // Outline.DrawBorder(Bounds);
        // base.Draw(gameTime);
    }
}

public enum SizingMode
{
    Fixed,
    ExpandToFit,
    FitContent
}

[Flags]
public enum AxisSizing
{
    FixedX = 1,
    FixedY = 2,
    ExpandX = 4,
    ExpandY = 8,
    ShrinkX = 16,
    ShrinkY = 32,
    FixedXFixedY = 3,
    ExpandXExpandY = 12,
    ShrinkXShrinkY = 48,
    FixedXExpandY = 9,
    FixedXShrinkY = 33,
    ExpandXShrinkY = 36,
    ExpandXFixedY = 6,
    ShrinkXFixedY = 19,
    ShrinkXExpandY = 24
}
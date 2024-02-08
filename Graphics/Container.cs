using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Roguelike.Graphics.Layout;
using Roguelike.UserInterface;
using Roguelike.Utility;

namespace Roguelike.Graphics;

public class Container : DrawableGameComponent
{
    public Alignment ContentAlignment = Alignment.Center;
    public IntVector2 ContentsSize = IntVector2.Zero;
    public Container Parent;
    public bool Debug = false;
    public RectangleShape Outline;
    public List<Container> Children = new();

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
    
    public IntVector2 LocalPosition
    {
        get => _localPosition;
        set
        {
            _localPosition = value;
            Outline.Bounds = Bounds;
            foreach (var child in Children)
            {
                child.Outline.Bounds = child.Bounds;
            }
        }
    }

    private IntVector2 _localPosition = IntVector2.Zero;

    public AxisSizing Sizing = AxisSizing.ShrinkXShrinkY;

    public IntVector2 Size
    {
        get
        {
            var x = Math.Max(_size.X, MinSize.X);
            var y = Math.Max(_size.Y, MinSize.Y);
            return new IntVector2(x, y);
        }
        set => _size = value;
    }
    
    private IntVector2 _size = IntVector2.Zero;

    public IntVector2 MinSize
    {
        get
        {
            var x = Math.Max(_minSize.X, ContentsSize.X);
            var y = Math.Max(_minSize.Y, ContentsSize.Y);
            return new IntVector2(x, y);
        }
        set => _minSize = value;
    }
    
    private IntVector2 _minSize = IntVector2.Zero;
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
            if (oldSize.X != Size.X)
            {
                ChangedSize?.Invoke(this);
            }

            if (oldSize.Y != Size.Y)
            {
                ChangedSize?.Invoke(this);
            }
        }
    }

    public delegate void SizeChangeEventHandler(Container sender);

    public event SizeChangeEventHandler ChangedSize;
    
    public Container(Game game) : base(game)
    {
        Outline = new RectangleShape(Game);
        Game.Components.Add(Outline);
    }
    
    public Container(Game game, Rectangle bounds) : this(game)
    {
        Bounds = bounds;
    }

    protected override void OnVisibleChanged(object sender, EventArgs args)
    {
        Outline.Visible = Visible;
        base.OnVisibleChanged(sender, args);
    }

    public void AddChild(Container e)
    {
        e.Parent = this;

        ChangedSize += e.OnParentChangedSize;
        e.ChangedSize += OnChildChangedSize;
        e.DrawOrder = DrawOrder + 1;
        Children.Add(e);
        Game.Components.Add(e);
    }

    public void RemoveChildren()
    {
        foreach (var child in Children)
        {
            child.RemoveChildren();
            Game.Components.Remove(child);
        }
        Children.Clear();
    }

    protected virtual void OnParentChangedSize(Container e)
    {
        if ((Sizing & (AxisSizing.ExpandX | AxisSizing.ExpandY)) != 0)
        {
            Parent.LayoutElements();
        }
    }

    public virtual void OnChildChangedSize(Container e)
    {
        if ((Sizing & (AxisSizing.ShrinkX | AxisSizing.ShrinkY)) != 0)
        {
            LayoutElements();
        }
    }
    
    public virtual void LayoutElements()
    {
        foreach (var child in Children)
        {
            // child.Position = Position + child.LocalPosition;
            child.LayoutElements();
        }
        AlignChildren();
    }

    protected virtual void AlignChildren()
    {
        // foreach (var child in Children)
        // {
        //     var offsetX = 0;
        //     if ((ContentAlignment & Alignment.Right) != 0)
        //     {
        //         offsetX = Size.X - child.Size.X;
        //     }  
        //     else if ((ContentAlignment & Alignment.Left) != 0)
        //     {
        //         offsetX = 0;
        //     } 
        //     else if ((ContentAlignment & Alignment.Center) != 0)
        //     {
        //         offsetX = (Size.X - child.Size.X) / 2;
        //     }
        //     
        //     var offsetY = 0;
        //     if ((ContentAlignment & Alignment.Bottom) != 0)
        //     {
        //         offsetY = Size.Y - child.Size.Y;
        //     }  
        //     else if ((ContentAlignment & Alignment.Top) != 0)
        //     {
        //         offsetY = 0;
        //     } 
        //     else if ((ContentAlignment & Alignment.Center) != 0)
        //     {
        //         offsetY = (Size.Y - child.Size.Y) / 2;
        //     }
        //
        //     child.LocalPosition += new IntVector2(offsetX, offsetY);
        // }
    }

    public override void Draw(GameTime gameTime)
    {
        if (Parent is null)
        {
            LayoutElements();
        }

        if (Debug)
        {
            Outline.DrawOrder = DrawOrder + 1;
            Outline.Bounds = Bounds;
            // if (Visible)
            // {
            //     Outline.Visible = true;
            // }
        }
        // else
        // {
        //     Outline.Visible = false;
        // }
        
        base.Draw(gameTime);
    }

    // public virtual void AddToBatch(SpriteBatch batch)
    // {
    //     foreach (var element in Children)
    //     {
    //         element.AddToBatch(batch);            
    //     };
    // }

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

[Flags]
public enum FlowDirection
{
    TopToBottom = 1,
    BottomToTop = 2,
    LeftToRight = 4,
    RightToLeft = 8,
    TopLeftToBottomRight = 5,
    BottomLeftToTopRight = 6,
    TopRightToBottomLeft = 9,
    BottomRightToTopLeft = 10
}

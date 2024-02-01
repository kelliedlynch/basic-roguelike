using System;
using System.Linq;
using Microsoft.Xna.Framework;

namespace Roguelike.Graphics.Layout;

public class VStackContainer : Container
{
    public FlowDirection FlowType = FlowDirection.TopToBottom;
    
    public VStackContainer(Game game) : base(game)
    {
    }

    public VStackContainer(Game game, Rectangle bounds) : base(game, bounds)
    {
    }

    public override void LayoutElements()
    {
        var x = 0;
        var y = 0;
        if (FlowType is FlowDirection.BottomToTop)
        {
            y = Size.Y;
        }

        var totalX = 0;
        var totalY = 0;
        var thisWidth = Math.Max(Math.Max(Size.X, MinSize.X), 1);
        var thisHeight = Math.Max(Math.Max(Size.Y, MinSize.Y), 1);;

        foreach (var child in Children)
        {
            if (child is Container c)
            {
                c.LayoutElements();
            }
            child.LocalPosition = new IntVector2(x, y);
            var childWidth = Math.Max(Math.Max(child.Size.X, child.MinSize.X), 1);
            var childHeight = Math.Max(Math.Max(child.Size.Y, child.MinSize.Y), 1);;
            if ((child.Sizing & AxisSizing.ExpandX) != 0)
            {
                childWidth = thisWidth;
            }
            
            if ((FlowType & FlowDirection.BottomToTop) != 0)
            {
                y -= child.Size.Y;
            }

            child.Size = new IntVector2(childWidth, childHeight);
            child.LocalPosition = new IntVector2(x, y);
            
            if ((FlowType & FlowDirection.TopToBottom) != 0)
            {
                y += child.Size.Y;
            }

            totalX = child.Size.X > totalX ? child.Size.X : totalX;
            totalY += child.Size.Y;
        }
        
        if ((Sizing & AxisSizing.ShrinkX) != 0)
        {
            thisWidth = totalX;
        }
        if ((Sizing & AxisSizing.ShrinkY) != 0)
        {
            thisHeight = totalY;
        }

        Size = new IntVector2(thisWidth, thisHeight);

        var unusedY = Size.Y - totalY;
        var toExpand = Children.Where(el => (el.Sizing & AxisSizing.ExpandY) != 0).ToList();
        if (toExpand.Count > 0)
        {
            var expandEach = unusedY / toExpand.Count;
            foreach (var element in toExpand)
            {
                unusedY -= expandEach;
                if (unusedY < expandEach)
                {
                    expandEach += unusedY;
                }
                element.Size = new IntVector2(element.Size.X, element.Size.Y + expandEach);
            }
        }


        // base.LayoutElements();
    }
}
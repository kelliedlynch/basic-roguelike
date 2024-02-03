using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Roguelike.UserInterface;
using Roguelike.Utility;

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

        var totalChildrenX = 0;
        var totalChildrenY = 0;
        
        foreach (var child in Children)
        {
            if (child is Container c)
            {
                c.LayoutElements();
            }
            child.LocalPosition = new IntVector2(x, y);
            var childWidth = Math.Max(child.Size.X, 1);
            var childHeight = Math.Max(child.Size.Y, 1);;
            
            if ((FlowType & FlowDirection.BottomToTop) != 0)
            {
                y -= childHeight;
            }

            child.Size = new IntVector2(childWidth, childHeight);
            child.LocalPosition = new IntVector2(x, y);
            
            if ((FlowType & FlowDirection.TopToBottom) != 0)
            {
                y += childHeight;
            }
            
            totalChildrenX = childWidth > totalChildrenX ? childWidth : totalChildrenX;
            totalChildrenY += childHeight;
        }
        
        var thisWidth = Math.Max(Size.X, 1);
        var thisHeight = Math.Max(Size.Y, 1);;
        
        if ((Sizing & AxisSizing.ShrinkX) != 0)
        {
            thisWidth = totalChildrenX;
        }
        if ((Sizing & AxisSizing.ShrinkY) != 0)
        {
            thisHeight = totalChildrenY;
        }

        Size = new IntVector2(thisWidth, thisHeight);

        var toExpandY = Children.Where(el => (el.Sizing & AxisSizing.ExpandY) != 0).ToList();
        if (toExpandY.Count > 0)
        {
            var unusedY = Size.Y - totalChildrenY;
            var expandEach = unusedY / toExpandY.Count;
            foreach (var element in toExpandY)
            {
                unusedY -= expandEach;
                if (unusedY < expandEach)
                {
                    expandEach += unusedY;
                }
                element.Size = new IntVector2(element.Size.X, element.Size.Y + expandEach);
            }

            totalChildrenY = Size.Y;
        }
        
        var toExpandX = Children.Where(el => (el.Sizing & AxisSizing.ExpandX) != 0).ToList();
        if (toExpandX.Count > 0)
        {
            foreach (var element in toExpandX)
            {
                element.Size = new IntVector2(Size.X, element.Size.Y);
            }

            totalChildrenX = Size.X;
        }

        ContentsSize = new IntVector2(totalChildrenX, totalChildrenY);
        
        AlignChildren();
    }

    protected override void AlignChildren()
    {
        var offsetY = 0;
        if ((ContentAlignment & Alignment.Bottom) != 0)
        {
            offsetY = Size.Y - ContentsSize.Y;
        }  
        else if ((ContentAlignment & Alignment.Top) != 0)
        {
            offsetY = 0;
        } 
        else if ((ContentAlignment & Alignment.Center) != 0)
        {
            offsetY = (Size.Y - ContentsSize.Y) / 2;
        }

        foreach (var child in Children)
        {
            var offsetX = 0;
            if ((ContentAlignment & Alignment.Right) != 0)
            {
                offsetX = Size.X - child.Size.X;
            }  
            else if ((ContentAlignment & Alignment.Left) != 0)
            {
                offsetX = 0;
            } 
            else if ((ContentAlignment & Alignment.Center) != 0)
            {
                offsetX = (Size.X - child.Size.X) / 2;
            }

            child.LocalPosition += new IntVector2(offsetX, offsetY);
        }
    }
}
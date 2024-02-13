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
            y = CalculatedSize.Y;
        }

        var totalChildrenX = 0;
        var totalChildrenY = 0;
        
        foreach (var child in Children)
        {
            child.LayoutElements();
            // child.LocalPosition = new IntVector2(x, y);
            var childWidth = Math.Max(child.CalculatedSize.X, 1);
            var childHeight = Math.Max(child.CalculatedSize.Y, 1);
            if ((child.Sizing & AxisSizing.ExpandY) != 0)
            {
                childHeight = Math.Min(childHeight, child.MinSize.Y);
            }
            
            if ((FlowType & FlowDirection.BottomToTop) != 0)
            {
                y -= childHeight;
            }

            child.LayoutSize = new IntVector2(childWidth, childHeight);
            // child.LocalPosition = new IntVector2(x, y);
            
            if ((FlowType & FlowDirection.TopToBottom) != 0)
            {
                y += childHeight;
            }
            
            totalChildrenX = childWidth > totalChildrenX ? childWidth : totalChildrenX;
            totalChildrenY += childHeight;
        }
        
        var thisWidth = Math.Max(CalculatedSize.X, 1);
        var thisHeight = Math.Max(CalculatedSize.Y, 1);;
        
        if ((Sizing & AxisSizing.ShrinkX) != 0)
        {
            thisWidth = totalChildrenX;
        }
        if ((Sizing & AxisSizing.ShrinkY) != 0)
        {
            thisHeight = totalChildrenY;
        }

        LayoutSize = new IntVector2(thisWidth, thisHeight);

        var toExpandY = Children.Where(el => (el.Sizing & AxisSizing.ExpandY) != 0).ToList();
        if (toExpandY.Count > 0)
        {
            var unusedY = AssignedSize.Y - totalChildrenY;
            var expandEach = unusedY / toExpandY.Count;
            foreach (var element in toExpandY)
            {
                unusedY -= expandEach;
                if (unusedY < expandEach)
                {
                    expandEach += unusedY;
                }
                element.LayoutSize = new IntVector2(element.LayoutSize.X, element.LayoutSize.Y + expandEach);
            }

            totalChildrenY = CalculatedSize.Y;
        }
        
        var toExpandX = Children.Where(el => (el.Sizing & AxisSizing.ExpandX) != 0).ToList();
        if (toExpandX.Count > 0)
        {
            foreach (var element in toExpandX)
            {
                element.LayoutSize = new IntVector2(CalculatedSize.X, element.CalculatedSize.Y);
            }

            totalChildrenX = CalculatedSize.X;
        }

        ContentsSize = new IntVector2(totalChildrenX, totalChildrenY);

        x = 0;
        y = 0;
        if (FlowType is FlowDirection.BottomToTop)
        {
            y = CalculatedSize.Y;
        }
        
        foreach (var child in Children)
        {
            if (FlowType is FlowDirection.BottomToTop)
            {
                y -= child.CalculatedSize.Y;
            }
            
            child.LocalPosition = new IntVector2(x, y);
            
            if ((FlowType & FlowDirection.TopToBottom) != 0)
            {
                y += child.CalculatedSize.Y;
            }
            
        }
        
        AlignChildren();
    }

    protected override void AlignChildren()
    {
        var offsetY = 0;
        if ((ContentAlignment & Alignment.Bottom) != 0)
        {
            offsetY = CalculatedSize.Y - ContentsSize.Y;
        }  
        else if ((ContentAlignment & Alignment.Top) != 0)
        {
            offsetY = 0;
        } 
        else if ((ContentAlignment & Alignment.Center) != 0)
        {
            offsetY = (CalculatedSize.Y - ContentsSize.Y) / 2;
        }

        foreach (var child in Children)
        {
            var offsetX = 0;
            if ((ContentAlignment & Alignment.Right) != 0)
            {
                offsetX = CalculatedSize.X - child.CalculatedSize.X;
            }  
            else if ((ContentAlignment & Alignment.Left) != 0)
            {
                offsetX = 0;
            } 
            else if ((ContentAlignment & Alignment.Center) != 0)
            {
                offsetX = (CalculatedSize.X - child.CalculatedSize.X) / 2;
            }

            child.LocalPosition += new IntVector2(offsetX, offsetY);
        }
    }
}
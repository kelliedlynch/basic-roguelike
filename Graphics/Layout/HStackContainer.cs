using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Roguelike.UserInterface;
using Roguelike.Utility;

namespace Roguelike.Graphics.Layout;

public class HStackContainer : Container
{
    public FlowDirection FlowType = FlowDirection.LeftToRight;
    
    public HStackContainer(Game game) : base(game)
    {
    }

    public HStackContainer(Game game, Rectangle bounds) : this(game)
    {
        
    }

    public override void LayoutElements()
    {
        var x = 0;
        var y = 0;
        var rtl = (FlowType & FlowDirection.RightToLeft) != 0;
        if (rtl)
        {
            x = AssignedSize.X;
        }

        var totalChildrenX = 0;
        var totalChildrenY = 0;
        
        foreach (var child in Children)
        {
            // if (child is Container c)
            // {
            //     c.LayoutElements();
            // }
            child.LocalPosition = new IntVector2(x, y);
            var childWidth = Math.Max(child.AssignedSize.X, 1);
            var childHeight = Math.Max(child.AssignedSize.Y, 1);;
            
            if (rtl)
            {
                x -= childWidth;
            }

            child.AssignedSize = new IntVector2(childWidth, childHeight);
            child.LocalPosition = new IntVector2(x, y);
            
            if (!rtl)
            {
                x += childWidth;
            }

            totalChildrenX += childWidth;
            totalChildrenY = childHeight > totalChildrenY ? childHeight : totalChildrenY;
        }
        
        var thisWidth = Math.Max(AssignedSize.X, 1);
        var thisHeight = Math.Max(AssignedSize.Y, 1);;
        
        if ((Sizing & AxisSizing.ShrinkX) != 0)
        {
            thisWidth = totalChildrenX;
        }
        if ((Sizing & AxisSizing.ShrinkY) != 0)
        {
            thisHeight = totalChildrenY;
        }

        AssignedSize = new IntVector2(thisWidth, thisHeight);

        var toExpandX = Children.Where(el => (el.Sizing & AxisSizing.ExpandX) != 0).ToList();
        if (toExpandX.Count > 0)
        {
            var unusedX = AssignedSize.X - totalChildrenX;
            var expandEach = unusedX / toExpandX.Count;
            foreach (var element in toExpandX)
            {
                unusedX -= expandEach;
                if (unusedX < expandEach)
                {
                    expandEach += unusedX;
                }
                element.AssignedSize = new IntVector2(element.AssignedSize.X + expandEach, element.AssignedSize.Y);
            }

            totalChildrenX = AssignedSize.X;
        }
        
        var toExpandY = Children.Where(el => (el.Sizing & AxisSizing.ExpandY) != 0).ToList();
        if (toExpandY.Count > 0)
        {
            foreach (var element in toExpandY)
            {
                element.AssignedSize = new IntVector2(element.AssignedSize.X, AssignedSize.Y);
            }

            totalChildrenY = AssignedSize.Y;
        }

        ContentsSize = new IntVector2(totalChildrenX, totalChildrenY);
        
        AlignChildren();
    }

    protected override void AlignChildren()
    {
        var offsetX = 0;
        if ((ContentAlignment & Alignment.Right) != 0)
        {
            offsetX = AssignedSize.X - ContentsSize.X;
        }  
        else if ((ContentAlignment & Alignment.Left) != 0)
        {
            offsetX = 0;
        } 
        else if ((ContentAlignment & Alignment.Center) != 0)
        {
            offsetX = (AssignedSize.X - ContentsSize.X) / 2;
        }

        foreach (var child in Children)
        {
            var offsetY = 0;
            if ((ContentAlignment & Alignment.Bottom) != 0)
            {
                offsetY = AssignedSize.Y - child.AssignedSize.Y;
            }  
            else if ((ContentAlignment & Alignment.Top) != 0)
            {
                offsetY = 0;
            } 
            else if ((ContentAlignment & Alignment.Center) != 0)
            {
                offsetY = (AssignedSize.Y - child.AssignedSize.Y) / 2;
            }

            child.LocalPosition += new IntVector2(offsetX, offsetY);
        }
    }
}
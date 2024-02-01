using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Roguelike.UserInterface;

namespace Roguelike.Graphics.Layout;

public class FlowContainer : Container
{
    public FlowDirection FlowType = FlowDirection.TopLeftToBottomRight;
    
    public FlowContainer(Game game) : base(game)
    {
    }

    public FlowContainer(Game game, Rectangle bounds) : base(game, bounds)
    {
    }

    public void PreliminaryLayout()
    {
        // Place all the elements in this container. their min size is MAX(element.MinSize, 1)
        var ltr = (FlowType & FlowDirection.LeftToRight) != 0;
        var rtl = (FlowType & FlowDirection.RightToLeft) != 0;
        var ttb = (FlowType & FlowDirection.TopToBottom) != 0;
        var btt = (FlowType & FlowDirection.BottomToTop) != 0;
        var x = 0;
        var y = 0;

        var size = Size;
        
        if (rtl)
        {
            x = size.X;
        }

        if (btt)
        {
            y = size.Y;
        }
        var originX = x;
        var originY = y;
        
        var layoutX = Math.Max(Math.Max(Size.X, MinSize.X), 1);
        var layoutY = Math.Max(Math.Max(Size.Y, MinSize.Y), 1);
        LayoutSize = new IntVector2(layoutX, layoutY);

        var totalElementWidth = 0;
        var totalElementHeight = 0;
        var biggestX = 0;
        var biggestY = 0;
        var newLine = false;
        var newCol = false;
        var childrenInRow = new List<DrawnElement>();
        var childrenInColumn = new List<DrawnElement>();
        foreach (var child in Children)
        {
            if (child is FlowContainer c)
            {
                if ((child.Sizing & (AxisSizing.ShrinkX | AxisSizing.ShrinkY)) != 0)
                {
                    c.LayoutElements();
                }
            }

            if (rtl)
            {
                x -= child.LayoutSize.X;
            }
            if (btt)
            {
                y -= child.LayoutSize.Y;
            }
            
            child.LocalPosition = new IntVector2(x, y);
            biggestX = child.LayoutSize.X > biggestX ? child.LayoutSize.X : biggestX;
            biggestY = child.LayoutSize.Y > biggestY ? child.LayoutSize.Y : biggestY;

            childrenInRow.Add(child);
            childrenInColumn.Add(child);
            // child.LayoutSize = new IntVector2();
            
            if (ltr)
            {
                x += child.LayoutSize.X;
                
                if (x > LayoutSize.X && (ttb || btt))
                {
                    newLine = true;
                }
            }
            else if (rtl)
            {
                if (x < 0 && (ttb || btt))
                {
                    newLine = true;
                }
            }
            
            if ((ttb || btt) && !(ltr || rtl))
            {
                totalElementWidth = biggestX > totalElementWidth ? biggestX : totalElementWidth;
                totalElementHeight += child.LayoutSize.Y;
            }
            
            if (newLine)
            {
                var unusedX = LayoutSize.X - x > 0 ? LayoutSize.X - x : 0;
                var expandEach = unusedX / childrenInRow.Count(el => (el.Sizing & AxisSizing.ExpandX) != 0);
                foreach (var rowChild in childrenInRow)
                {
                    var rowChildHeight = rowChild.LayoutSize.Y;
                    var rowChildWidth = rowChild.LayoutSize.X;
                    if ((rowChild.Sizing & AxisSizing.ExpandY) != 0)
                    {
                        rowChildHeight = biggestY;
                    }
                    if ((rowChild.Sizing & AxisSizing.ExpandX) != 0)
                    {
                        rowChildWidth += expandEach;
                        unusedX -= expandEach;
                        if (unusedX < expandEach) rowChildWidth += unusedX;
                    }

                    rowChild.LayoutSize = new IntVector2(rowChildHeight, rowChildWidth);
                    
                }
                childrenInRow.Clear();
                
                x = originX;
                totalElementHeight += biggestY;
                if ((FlowType & FlowDirection.TopToBottom) != 0)
                {
                    y += biggestY;
                }
                else if ((FlowType & FlowDirection.BottomToTop) != 0)
                {
                    
                    y -= biggestY;
                }
                biggestY = 0;
                newLine = false;
            }
            else
            {
                if (ttb)
                {
                    y += child.LayoutSize.Y;

                    if (y > LayoutSize.Y && (ltr || rtl))
                    {
                        newCol = true;
                    }
                }
                else if (btt)
                {
                    if (y < 0 && (ltr || rtl))
                    {
                        newCol = true;
                    }
                }
            }
            
            if ((ltr || rtl) && !(ttb || btt))
            {
                totalElementWidth += child.LayoutSize.X;
                totalElementHeight = biggestY > totalElementHeight ? biggestY : totalElementHeight;
            }
            
            if (newCol)
            {
                var unusedY = LayoutSize.Y - y > 0 ? LayoutSize.Y - y : 0;
                var expandEach = unusedY / childrenInColumn.Count(el => (el.Sizing & AxisSizing.ExpandY) != 0);
                foreach (var columnChild in childrenInColumn)
                {
                    var columnChildHeight = columnChild.LayoutSize.Y;
                    var columnChildWidth = columnChild.LayoutSize.X;
                    if ((columnChild.Sizing & AxisSizing.ExpandX) != 0)
                    {
                        columnChildWidth = biggestX;
                    }
                    if ((columnChild.Sizing & AxisSizing.ExpandY) != 0)
                    {
                        columnChildHeight += expandEach;
                        unusedY -= expandEach;
                    }

                    columnChild.LayoutSize = new IntVector2(columnChildHeight, columnChildWidth);
                }

                childrenInColumn[^1].LayoutSize += new IntVector2(0, unusedY);
                childrenInColumn.Clear();
                
                y = originY;
                totalElementWidth += biggestX;
                if ((FlowType & FlowDirection.LeftToRight) != 0)
                {
                    x += biggestX;
                }
                else if ((FlowType & FlowDirection.RightToLeft) != 0)
                {
                    x -= biggestX;
                }
                biggestX = 0;
                newCol = false;
            }
        }

        ContentsSize = new IntVector2(totalElementWidth, totalElementHeight);
        
        if ((Sizing & AxisSizing.ShrinkX) != 0)
        {
            layoutX = ContentsSize.X;
        }
        if ((Sizing & AxisSizing.ShrinkY) != 0)
        {
            layoutY = ContentsSize.Y;
        }

        LayoutSize = new IntVector2(layoutX, layoutY);
    }

    private void SetThisSize()
    {
        var sizeX = LayoutSize.X;
        var sizeY = LayoutSize.Y;
        if (Sizing != (AxisSizing.FixedX | AxisSizing.FixedY))
        {
            if ((Sizing & AxisSizing.ShrinkX) != 0)
            {
                sizeX = ContentsSize.X;
            }
            if ((Sizing & AxisSizing.ShrinkY) != 0)
            {
                sizeY = ContentsSize.Y;
            }
        }

        Size = new IntVector2(sizeX, sizeY);
    }

    private void RecalculateLayout()
    {


    }

    private void ResizeElements()
    {
        // for child in children
        //      ResizeElements()
        // if this is shrink, shrink to allElementsSize (size of children or minsize)
        // if this is expand, expand to parent size (parent must have a size at this point)
    }

    public override void LayoutElements()
    {
        PreliminaryLayout();
        // foreach (var child in Children)
        // {
        //     if (child is FlowContainer c)
        //     {
        //         c.PreliminaryLayout();
        //     }
        // }
        // if ((Sizing & AxisSizing.ShrinkX) != 0)
        // {
        //     layoutX = ContentsSize.X;
        // }
        // if ((Sizing & AxisSizing.ShrinkY) != 0)
        // {
        //     layoutY = ContentsSize.Y;
        // }
        //
        // LayoutSize = new IntVector2(layoutX, layoutY);
        
        // SetThisSize();
        // RecalculateLayout();
    }
    
    // public override void LayoutElements()
    // {
    //     var ltr = (FlowType & FlowDirection.LeftToRight) != 0;
    //     var rtl = (FlowType & FlowDirection.RightToLeft) != 0;
    //     var ttb = (FlowType & FlowDirection.TopToBottom) != 0;
    //     var btt = (FlowType & FlowDirection.BottomToTop) != 0;
    //     var x = 0;
    //     var y = 0;
    //
    //     var size = Size;
    //     
    //     if (rtl)
    //     {
    //         x = size.X;
    //     }
    //
    //     if (btt)
    //     {
    //         y = size.Y;
    //     }
    //     var originX = x;
    //     var originY = y;
    //
    //     var totalElementWidth = 0;
    //     var totalElementHeight = 0;
    //     var biggestX = 0;
    //     var biggestY = 0;
    //     var newLine = false;
    //     var newCol = false;
    //     var childrenInRow = new List<DrawnElement>();
    //     var childrenInColumn = new List<DrawnElement>();
    //     
    //     //
    //     // IF THIS IS SHRINKX OR SHRINKY: WE NEED CHILD SIZES BEFORE SETTING THIS SIZE
    //     //      FOR EACH CHILD:
    //     //          IF CHILD IS SHRINKX OR SHRINKY: CHILD MUST LAYOUT ITS ELEMENTS BEFORE KNOWING ITS SIZE
    //     //              CHILD WILL SET ITS OWN SIZE AFTER LAYING OUT ELEMENTS
    //     //          IF CHILD IS EXPANDX OR EXPANDY: CHILD NEEDS THIS SIZE BEFORE SETTING ITS SIZE
    //     //              SET CHILD SIZE TO 1 OR MIN AND CALCULATE LATER
    //     //      NOW CHILDREN SHOULD ALL HAVE SIZES
    //
    //     // TIME TO SET THIS SIZE
    //     //      IF THIS IS FIXEDX AND FIXEDY, WE KNOW THIS SIZE
    //     //      IF THIS IS EXPANDX OR EXPANDY: WE NEED PARENT SIZE BEFORE SETTING THIS SIZE
    //     //          OH NO, PARENT HAS NOT SET ITS SIZE YET
    //     //          OR HAS IT? I AM CONFUSED
    //     //      IF THIS IS SHRINKX OR SHRINKY: WE CAN CALDULATE THIS SIZE BECAUSE CHILDREN HAVE SIZES
    //     //          LAYOUT CHILDREN
    //
    //
    //     
    //     
    //     //      IF CHILD IS FIXEDX AND FIXEDY, WE KNOW ITS SIZE
    //     //      IF CHILD IS EXPANDX OR EXPANDY, WE NEED TO DETERMINE ITS SIZE
    //     // IF THIS IS EXPANDX AND EXPANDY: THIS SIZE IS PARENT SIZE, CHILD SIZE IRRELEVANT
    //     // IF THIS IS FIXEDX AND FIXEDY: THIS SIZE IS FIXED SIZE, CHILD SIZE IRRELEVANT
    //     // IF THIS IS EXPANDXSHRINKY OR EXPANDYSHRINKX: THIS EXPAND DIM IS PARENT SIZE, WE NEED CHILD SIZES FOR SHRINK DIM
    //     //
    //     
    //     foreach (var child in Children)
    //     {
    //         if (child is Container c)
    //         {
    //             if ((child.Sizing & (AxisSizing.ShrinkX | AxisSizing.ShrinkY)) != 0)
    //             {
    //                 c.LayoutElements();
    //             }
    //         }
    //         
    //         var layoutX = child.Size.X;
    //         var layoutY = child.Size.Y;
    //         if ((child.Sizing & AxisSizing.ExpandX) != 0)
    //         {
    //             layoutX = child.MinSize.X > 0 ? child.MinSize.X : 1;
    //             
    //         }
    //
    //         if ((child.Sizing & AxisSizing.ExpandY) != 0)
    //         {
    //             layoutY = child.MinSize.Y > 0 ? child.MinSize.Y : 1;
    //         }
    //
    //         var layoutSize = new IntVector2(layoutX, layoutY);
    //
    //         if (rtl)
    //         {
    //             x -= layoutSize.X > 0 ? layoutSize.X : 1;
    //         }
    //         if (btt)
    //         {
    //             y -= layoutSize.Y > 0 ? layoutSize.Y : 1;
    //         }
    //         
    //         child.LocalPosition = new IntVector2(x, y);
    //         biggestX = layoutSize.X > biggestX ? layoutSize.X : biggestX;
    //         biggestY = layoutSize.Y > biggestY ? layoutSize.Y : biggestY;
    //
    //         childrenInRow.Add(child);
    //         childrenInColumn.Add(child);
    //         child.LayoutSize = layoutSize;
    //         
    //         if (ltr)
    //         {
    //             x += layoutSize.X;
    //             
    //             if (x > size.X && (ttb || btt))
    //             {
    //                 newLine = true;
    //             }
    //         }
    //         else if (rtl)
    //         {
    //             if (x < 0 && (ttb || btt))
    //             {
    //                 newLine = true;
    //             }
    //         }
    //         
    //         if ((ttb || btt) && !(ltr || rtl))
    //         {
    //             totalElementHeight += layoutSize.Y;
    //             totalElementWidth = biggestX > totalElementWidth ? biggestX : totalElementWidth;
    //         }
    //         
    //         if (newLine)
    //         {
    //             var unusedX = Size.X - x > 0 ? Size.X - x : 0;
    //             var expandEach = unusedX / childrenInRow.Count(el => (el.Sizing & AxisSizing.ExpandX) != 0);
    //             foreach (var rowChild in childrenInRow)
    //             {
    //                 var rowChildHeight = rowChild.LayoutSize.Y;
    //                 var rowChildWidth = rowChild.LayoutSize.X;
    //                 if ((rowChild.Sizing & AxisSizing.ExpandY) != 0)
    //                 {
    //                     rowChildHeight = biggestY;
    //                 }
    //                 if ((rowChild.Sizing & AxisSizing.ExpandX) != 0)
    //                 {
    //                     rowChildWidth += expandEach;
    //                     unusedX -= expandEach;
    //                 }
    //
    //                 rowChild.Size = new IntVector2(rowChildHeight, rowChildWidth);
    //             }
    //
    //             childrenInRow[^1].Size += new IntVector2(unusedX, 0);
    //             childrenInRow.Clear();
    //             
    //             x = originX;
    //             totalElementHeight += biggestY;
    //             if ((FlowType & FlowDirection.TopToBottom) != 0)
    //             {
    //                 y += biggestY;
    //             }
    //             else if ((FlowType & FlowDirection.BottomToTop) != 0)
    //             {
    //                 
    //                 y -= biggestY;
    //             }
    //             biggestY = 0;
    //             newLine = false;
    //         }
    //         
    //         
    //         if (ttb)
    //         {
    //             // if (!ltr && !rtl)
    //             // {
    //                 y += layoutSize.Y;
    //             // }
    //
    //             if (y > size.Y && (ltr || rtl))
    //             {
    //                 newCol = true;
    //             }
    //         }
    //         else if (btt)
    //         {
    //             if (y < 0 && (ltr || rtl))
    //             {
    //                 newCol = true;
    //             }
    //         }
    //
    //         
    //         
    //         if ((ltr || rtl) && !(ttb || btt))
    //         {
    //             totalElementWidth += child.LayoutSize.X;
    //             totalElementHeight = biggestY > totalElementHeight ? biggestY : totalElementHeight;
    //         }
    //         
    //
    //         if (newCol)
    //         {
    //             var unusedY = Size.Y - y > 0 ? Size.Y - y : 0;
    //             var expandEach = unusedY / childrenInColumn.Count(el => (el.Sizing & AxisSizing.ExpandY) != 0);
    //             foreach (var columnChild in childrenInColumn)
    //             {
    //                 var columnChildHeight = columnChild.LayoutSize.Y;
    //                 var columnChildWidth = columnChild.LayoutSize.X;
    //                 if ((columnChild.Sizing & AxisSizing.ExpandX) != 0)
    //                 {
    //                     columnChildWidth = biggestX;
    //                 }
    //                 if ((columnChild.Sizing & AxisSizing.ExpandY) != 0)
    //                 {
    //                     columnChildHeight += expandEach;
    //                     unusedY -= expandEach;
    //                 }
    //
    //                 columnChild.Size = new IntVector2(columnChildHeight, columnChildWidth);
    //             }
    //
    //             childrenInColumn[^1].Size += new IntVector2(0, unusedY);
    //             childrenInColumn.Clear();
    //             
    //             y = originY;
    //             totalElementWidth += biggestX;
    //             if ((FlowType & FlowDirection.LeftToRight) != 0)
    //             {
    //                 x += biggestX;
    //             }
    //             else if ((FlowType & FlowDirection.RightToLeft) != 0)
    //             {
    //                 x -= biggestX;
    //             }
    //             biggestX = 0;
    //             newCol = false;
    //         }
    //
    //         // if (child is Container expC && child.Sizing != SizingMode.FitContent)
    //         // {
    //         //     expC.LayoutElements();
    //         // }
    //
    //     }
    //
    //     var allElementsSize = new IntVector2(totalElementWidth, totalElementHeight);
    //     // var padding = IntVector2.Zero;
    //     // if (Sizing == SizingMode.FitContent)
    //     // {
    //     //     // size = allElementsSize;
    //     //     Size = allElementsSize;
    //     // }
    //
    //     var padding = (Size - allElementsSize) / IntVector2.Two;
    //     var addX = padding.X;
    //     var addY = padding.Y;
    //
    //
    //     if ((ContentAlignment & Alignment.Top) != 0)
    //     {
    //         addY = 0;
    //     }
    //
    //     if ((ContentAlignment & Alignment.Bottom) != 0)
    //     {
    //         addY *= 2;
    //     }
    //
    //     if ((ContentAlignment & Alignment.Left) != 0)
    //     {
    //         addX = 0;
    //     }
    //
    //     if ((ContentAlignment & Alignment.Right) != 0)
    //     {
    //         addX *= 2;
    //     }
    //     
    //     foreach (var child in Children)
    //     {
    //         child.LocalPosition += new IntVector2(addX, addY);
    //     }
    //     // base.LayoutElements();
    // }
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
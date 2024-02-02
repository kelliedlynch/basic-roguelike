using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Roguelike.UserInterface;
using Roguelike.Utility;
using Vector2 = System.Numerics.Vector2;

namespace Roguelike;

public class ActivityLog : DialogBox
{
    public List<string> Messages = new();

    public ActivityLog(RoguelikeGame game) : base(game)
    {
        Visible = false;
    }

    public void InitializeLog()
    {
        Messages.Clear();
    }
    
    public void LogEvent(object sender, EventArgs args)
    {
        var a = (ActivityLogEventArgs)args;
        Messages.Add(a.Message);
    }

    protected void DrawText(SpriteBatch spriteBatch)
    {
        var textHeight = TileSize.Y;
        var lineNumber = 1;
        var lineHeight = (int)TextLabel.Font.MeasureString("Q").Y;
        var linePadding = 2;
        while (textHeight < Size.Y * TileSize.Y)
        {
            if (Messages.Count < lineNumber) break;
            var loc = new IntVector2(Position.X + TileSize.X,
                Position.Y + (Size.Y + 2) * TileSize.Y - textHeight - lineHeight);
            spriteBatch.DrawString(TextLabel.Font, Messages[^lineNumber],loc, Color.White);
            textHeight += lineHeight + linePadding;
            lineNumber++;
        }
    }
}

public class ActivityLogEventArgs : EventArgs
{
    public string Message;

    public ActivityLogEventArgs(string m)
    {
        Message = m;
    }
}
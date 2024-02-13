using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Roguelike.Graphics;
using Roguelike.UserInterface;
using Roguelike.Utility;
using Vector2 = System.Numerics.Vector2;

namespace Roguelike;

public class ActivityLog : DialogBox
{
    public List<string> Messages = new();
    public int LineSpacing = 2;

    public ActivityLog(Game game) : base(game)
    {
        ContentAlignment = Alignment.BottomLeft;
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
    
    protected void BuildText()
    {
        var lineHeight = (int)TextLabel.Font.MeasureString("Q").Y;
        var lineNumber = 1;
        var textHeight = 0;
        // var currentY = Size.Y - PaddingBottom - LineSpacing;
        //
        // var spriteBatch = Game.Services.GetService<SpriteBatch>();
        TextLabel.Text = "";
        while (textHeight < CalculatedSize.Y - PaddingTop - PaddingBottom)
        {
            if (Messages.Count < lineNumber) break;
            // currentY -= (lineHeight + LineSpacing) * lineNumber;
            // var loc = new IntVector2(PaddingLeft, currentY);
            // TODO: CURRENTLY THIS ONLY WORKS FOR MESSAGES THAT TAKE UP ONLY ONE LINE
            //      ADD IN WORD WRAPPING AND DYNAMIC LINE SIZING
            // spriteBatch.DrawString(TextLabel.Font, Messages[^lineNumber],loc, Color.White);
            TextLabel.Text += "\n" + Messages[^lineNumber];
            textHeight += lineHeight + LineSpacing;
            lineNumber++;
        }
        LayoutElements();
    }


    public override void Draw(GameTime gameTime)
    {
        BuildText();
        base.Draw(gameTime);
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
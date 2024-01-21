using System;
using Microsoft.Xna.Framework;

namespace Roguelike.Entity;

public class Money : Entity
{
    public int Value;

    public EventHandler EntityAddedToWorld;

    public Money(int value)
    {
        Value = value;
        SpriteLocation = new IntVector2(22, 4);
        Color = Color.Gold;
    }
}
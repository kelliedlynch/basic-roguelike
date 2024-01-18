using System;
using Microsoft.Xna.Framework;

namespace Roguelike;

public struct IntVector2 : IEquatable<IntVector2>
{
    public int X;
    public int Y;

    public IntVector2(int x, int y)
    {
        X = x;
        Y = y;
    }

    public bool Equals(IntVector2 other)
    {
        return _equalsIntVector(other);
    }

    public override bool Equals(object obj)
    {
        if (obj is IntVector2)
        {
            return _equalsIntVector((IntVector2)obj);
        }

        if (obj is Vector2)
        {
            return _equalsXNAVector((Vector2)obj);
        }

        return false;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(X * 10000, Y);
    }

    private bool _equalsIntVector(IntVector2 obj)
    {
        return obj.X == X && obj.Y == Y;
    }

    private bool _equalsXNAVector(Vector2 obj)
    {
        var diffX = Math.Abs(obj.X - X);
        var diffY = Math.Abs(obj.Y - Y);
        return diffX < .00001f && diffY < .00001f;
    }

    public static bool operator ==(IntVector2 left, object right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(IntVector2 left, object right)
    {
        return !(left == right);
    }
    
    public static IntVector2 operator +(IntVector2 l, IntVector2 r)
    {
        return new IntVector2(l.X + r.X, l.Y + r.Y);
    }
    
    public static IntVector2 operator *(IntVector2 l, IntVector2 r)
    {
        return new IntVector2(l.X * r.X, l.Y * r.Y);
    }
    
    public static IntVector2 operator -(IntVector2 l, IntVector2 r)
    {
        return new IntVector2(l.X - r.X, l.Y - r.Y);
    }
    
    public static IntVector2 operator /(IntVector2 l, IntVector2 r)
    {
        return new IntVector2(l.X / r.X, l.Y / r.Y);
    }
}
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Roguelike.Content.Entity;

public class Entity
{
    public IntVector2 Location;
    // protected Texture2D SpriteSheet;
    public string SpriteSheet = "Graphics/monochrome-transparent_packed";
    public IntVector2 SpriteLocation;
    public Color Color = Color.Beige;

    public event EventHandler EntityWasDestroyed;

    public virtual void Destroy()
    {
        EntityWasDestroyed?.Invoke(this, new DestroyEventArgs(this));
    }
}

public class DestroyEventArgs : EventArgs
{
    public Entity Entity;

    public DestroyEventArgs(Entity e)
    {
        Entity = e;
    }
}
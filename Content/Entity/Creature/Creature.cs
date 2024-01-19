namespace Roguelike.Content.Entity.Creature;

public class Creature : Entity
{
    public Pathfinder Pathfinder = new ();

    public Creature()
    {
        SpriteLocation = new IntVector2(25, 2);
    }
}
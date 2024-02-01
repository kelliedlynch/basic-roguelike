using System.Collections.Generic;
using Roguelike.Entity.Item;

namespace Roguelike.Entity.Creature;

public class Inventory
{
    public Weapon Weapon;
    public List<Item.Item> Bag = new();

    public List<Item.Item> AllItems
    {
        get
        {
            var items = new List<Item.Item>();
            if (Weapon is not null)
            {
                items.Add(Weapon);
            }
            items.AddRange(Bag);
            return items;
        }
    }

    public void EquipItem(Item.Item item)
    {
        if (item is Weapon w)
        {
            if (Weapon is not null)
            {
                Bag.Add(Weapon);
            }

            Weapon = w;
        }
    }
}
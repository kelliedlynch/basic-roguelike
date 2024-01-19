using System.Collections.Generic;
using JetBrains.Annotations;
using Roguelike.Content.Entity;

namespace Roguelike;

public class TileMap
{
    public readonly int Width;
    public readonly int Height;
    public readonly DungeonTile[,] Tiles;
    public readonly List<Feature>[,] Features;
    public StairsDown StairsDown;
    public StairsUp StairsUp;
    public IntVector2 EntryPoint;
    
    public TileMap(int width, int height)
    {
        Width = width;
        Height = height;
        Tiles = new DungeonTile[Width, Height];
        Features = new List<Feature>[Width, Height];
        
        for (var i = 0; i < width; i++)
        {
            for (var j = 0; j < height; j++)
            {
                Tiles[i, j] = new DungeonTile(i, j);
                Features[i, j] = new List<Feature>();
            }
        }
    }
    
    [UsedImplicitly]
    public List<DungeonTile> GetAdjacentTiles(DungeonTile tile, int adjacencyType = 0)
    {
        return GetAdjacentTiles(new IntVector2(tile.X, tile.Y), adjacencyType);
    }

    [UsedImplicitly]
    public List<DungeonTile> GetAdjacentTiles(IntVector2 location, int adjacencyType = 0)
    {
        // adjacencyType: 0 orthogonal, 1 diagonal, 2 both
        
        var temp = new List<DungeonTile>();

        var row = location.Y;
        var col = location.X;

        if (adjacencyType is 0 or 2)
        {
            if(row + 1 < Height)
            {
                temp.Add(Tiles[col, row + 1]);
            }
            if(row - 1 >= 0)
            {
                temp.Add(Tiles[col, row - 1]);
            }
            if(col - 1 >= 0)
            {
                temp.Add(Tiles[col - 1, row]);
            }
            if(col + 1 < Width)
            {
                temp.Add(Tiles[col + 1, row]);
            }
        }

        if (adjacencyType is 1 or 2)
        {
            if (row + 1 < Height && col + 1 < Width)
            {
                temp.Add(Tiles[col + 1, row + 1]);
            }
            if (row - 1 >= 0 && col + 1 < Width)
            {
                temp.Add(Tiles[col + 1, row - 1]);
            }
            if (row + 1 < Height && col - 1 >= 0)
            {
                temp.Add(Tiles[col - 1, row + 1]);
            }
            if (row - 1 >= 0 && col - 1 >= 0)
            {
                temp.Add(Tiles[col - 1, row - 1]);
            }
            
        }


        return temp;    }

    public DungeonTile GetTileAt(IntVector2 pos)
    {
        return Tiles[pos.X, pos.Y];
    }
}
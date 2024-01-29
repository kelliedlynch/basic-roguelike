using System;
using System.Collections.Generic;
using Roguelike.Entity.Feature;

namespace Roguelike.Map;

public class TileMap
{
    public readonly int Width;
    public readonly int Height;
    public readonly int LevelNumber;
    public readonly DungeonTile[,] Tiles;
    public StairsDown StairsDown;
    public StairsUp StairsUp;

    private readonly Random _random = new();
    
    public TileMap(int width, int height, int level)
    {
        Width = width;
        Height = height;
        LevelNumber = level;
        Tiles = new DungeonTile[Width, Height];
        
        for (var i = 0; i < width; i++)
        {
            for (var j = 0; j < height; j++)
            {
                Tiles[i, j] = new DungeonTile(i, j, level);
            }
        }
    }
    
    public List<DungeonTile> GetAdjacentTiles(DungeonTile tile, DirectionType adjacencyType = DirectionType.Octilinear)
    {
        return GetAdjacentTiles(new IntVector2(tile.X, tile.Y), adjacencyType);
    }

    public List<DungeonTile> GetAdjacentTiles(IntVector2 location, DirectionType adjacencyType = DirectionType.Octilinear)
    {
        var temp = new List<DungeonTile>();

        var row = location.Y;
        var col = location.X;

        if (adjacencyType is DirectionType.Orthogonal or DirectionType.Octilinear)
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

        if (adjacencyType is DirectionType.Diagonal or DirectionType.Octilinear)
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


        return temp;    
    }

    public DungeonTile RandomAdjacentTile(IntVector2 pos, DirectionType adjacencyType = DirectionType.Octilinear)
    {
        
        var tiles = GetAdjacentTiles(pos, adjacencyType);
        var index = _random.Next(tiles.Count - 1);
        return tiles[index];
    }

    public DungeonTile GetTileAt(IntVector2 pos)
    {
        return Tiles[pos.X, pos.Y];
    }
}
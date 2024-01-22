using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Roguelike.Entity.Feature;

namespace Roguelike;

public class MapGenerator
{
    private readonly Random _random = new();
    private readonly IntVector2 _mapSize = new IntVector2(50, 68);
    private readonly IntVector2 _maxRoomRegionSize = new(14, 18);
    private readonly IntVector2 _minRoomSize = new(3, 3);
    
    // Creates a dungeon-style map, with rectangular rooms and narrow passages connecting them
    public TileMap GenerateDungeonMap()
    {
        var map = new TileMap(_mapSize.X, _mapSize.Y);
        
        // Divide grid into regions that could contain rooms
        var currentOrigin = new IntVector2(0, 0);
        var currentRect = new Rectangle(currentOrigin.X, currentOrigin.Y, _mapSize.X, _mapSize.Y);
        var allRects = new List<Rectangle> { currentRect };

        List<Rectangle> DivideRect(Rectangle rect)
        {
            var biggerDim = Math.Max(rect.Width, rect.Height);
            var divideAt = _random.Next((int)(0.25 * biggerDim), (int)(0.75 * biggerDim));
            var outputRects = new List<Rectangle>();
            if (rect.Width > rect.Height)
            {
                outputRects.Add(new Rectangle(rect.X, rect.Y, divideAt, rect.Height));
                outputRects.Add(new Rectangle(rect.X + divideAt, rect.Y, rect.Width - divideAt, rect.Height));
            }
            else
            {
                outputRects.Add(new Rectangle(rect.X, rect.Y, rect.Width, divideAt));
                outputRects.Add(new Rectangle(rect.X, rect.Y + divideAt, rect.Width, rect.Height - divideAt));
            }

            return outputRects;
        }


        while (true)
        {
            var dividedRects = new List<Rectangle>();
            foreach (var rect in allRects)
            {
                if (rect.Width < _maxRoomRegionSize.X && rect.Height < _maxRoomRegionSize.Y)
                {
                    dividedRects.Add(rect);
                    continue;
                }
                var div = DivideRect(rect);
                if (div.Count == 1)
                {
                    dividedRects.Add(rect);
                    continue;
                }
                dividedRects.AddRange(div);
            }

            if (dividedRects.Count == allRects.Count)
            {
                break;
            }
            allRects = dividedRects;
        }
        
        // In each rectangle, attempt to place a room
        
        // "center" is not the center of a room, it's just a random tile inside the room that serves as its "base"
        var roomCenters = new List<IntVector2>();
        foreach (var rect in allRects)
        {
            var leftOffset = _random.Next(1, 3);
            var rightOffset = _random.Next(1, 3);
            var topOffset = _random.Next(1, 4);
            var bottomOffset = _random.Next(1, 4);
            if (rect.Width - leftOffset - rightOffset < _minRoomSize.X || rect.Height - topOffset - bottomOffset < _minRoomSize.Y)
            {
                continue;
            }

            var left = rect.X + leftOffset;
            var right = rect.X + rect.Width - rightOffset;
            var top = rect.Y + topOffset;
            var bottom = rect.Y + rect.Height - bottomOffset;

            var randomTile = new IntVector2(_random.Next(left + 1, right - 1), _random.Next(top + 1, bottom - 1));
            roomCenters.Add(randomTile);
            for (var i = left; i < right; i++)
            {
                for (var j = top; j < bottom; j++)
                {
                    map.Tiles[i, j].Type = TileType.Floor;

                }
            }
        }
        PutUpWalls(map);
        
        // Pick a random room center, check if it can reach every other center. 
        // The first time it can't reach one, carve a new path.
        // Repeat until we make it through the loop (one room can see every other room)
        var walkingPathfinder = new Pathfinder();
        var tunnelingPathfinder = new Pathfinder();
        tunnelingPathfinder.SetTilePassable(TileType.Wall, true);
        tunnelingPathfinder.SetTilePassable(TileType.Void, true);
        IntVector2 FindUnreachableRoom(IntVector2 origin)
        {
            foreach (var center in roomCenters)
            {
                var walkingPath = walkingPathfinder.FindPath(map, origin, center);
                if (walkingPath == null)
                {
                    return center;
                }
            }

            return origin;
        }

        while (true)
        {
            var i = _random.Next(roomCenters.Count);
            var origin = roomCenters[i];
            var unreachable = FindUnreachableRoom(origin);
            if (unreachable == origin)
            {
                break;
            }

            var path = tunnelingPathfinder.FindPath(map, origin, unreachable);
            // TODO: HANDLE WHEN PATH IS NOT FOUND
            foreach (var tile in path)
            {
                tile.Type = TileType.Floor;
            }
        }
        PutUpWalls(map);
        
        // Place stairs
        PlaceStairsUp(map);
        PlaceStairsDown(map);

        // SetDefaultEntryPoint(map);

        return map;
    }

    private bool IsValidStairsLocation(TileMap map, DungeonTile tile)
    {
        return IsValidStairsLocation(map, tile.Location);
    }
    
    private bool IsValidStairsLocation(TileMap map, IntVector2 location)
    { 
        var tile = map.GetTileAt(location); 
        if (tile.Type is TileType.Void or TileType.Wall
            || tile.X < 3
            || tile.X > map.Width - 3
            || tile.Y < 3
            || tile.Y > map.Height - 3
            || (tile.X > map.Width / 2 - 3 && tile.X < map.Width / 2 + 3)
            || (tile.Y > map.Height / 2 - 3 && tile.Y < map.Height / 2 + 3))
    {
        return false;
    }
    var adj = map.GetAdjacentTiles(tile.Location, 2);
    return adj.TrueForAll(x => x.Type == TileType.Floor);
    }

    private void PlaceStairsUp(TileMap map)
    {
        var attempts = 0;
        do
        {
            var w = _random.Next(3, map.Width - 3);
            var h = _random.Next(3, map.Height - 3);
            var tile = map.GetTileAt(new IntVector2(w, h));
                if (!IsValidStairsLocation(map, tile))
                {
                    attempts++;
                    continue;
                }

                map.StairsUp = new StairsUp(tile.X, tile.Y);
                map.Features[tile.X, tile.Y].Add(map.StairsUp);
                return;
        } while (attempts < 200);

        throw new ValidLocationNotFoundException("Could not place stairs up");
    }
    
    private void PlaceStairsDown(TileMap map)
    {
        var attempts = 0;
        do
        {
            var w = _random.Next(3, map.Width - 3);
            var h = _random.Next(3, map.Height - 3);
            var tile = map.GetTileAt(new IntVector2(w, h));
            if (!IsValidStairsLocation(map, tile))
            {
                attempts++;
                continue;
            }
            var pf = new Pathfinder();
            var distance = pf.FindPath(map, tile.Location, map.StairsUp.Location);
            if (distance is null) continue;
            var allowableDistance = (int)(map.Width * .4) + (int)(map.Height * .4);
            if (distance.Count < allowableDistance)
            {
                attempts++;
                continue;
            }
            
            map.StairsDown = new StairsDown(tile.Location.X, tile.Location.Y);
            map.Features[tile.Location.X, tile.Location.Y].Add(map.StairsDown);
            return;

        } while (attempts < 200);
        throw new ValidLocationNotFoundException("Could not place stairs down");
    }
    
    private void PutUpWalls(TileMap map)
    {
        foreach (var tile in map.Tiles)
        {
            foreach (var adjTile in map.GetAdjacentTiles(tile, 2))
            {
                if (tile.Type == TileType.Floor && adjTile.Type == TileType.Void)
                {
                    adjTile.Type = TileType.Wall;
                }
            }            
        }
    }
    
    // public void SetDefaultEntryPoint(TileMap map)
    // {
    //     map.EntryPoint = map.RandomAdjacentTile(map.StairsUp.Location).Location;
    // }
    
}

public class ValidLocationNotFoundException : SystemException
{
    public ValidLocationNotFoundException(string message)
    {
        Console.WriteLine(message);
    }
}
using System;
using System.Collections.Generic;
using Roguelike.Map;


// Based on A* Implementation by Dave Cusatis https://github.com/davecusatis/A-Star-Sharp

namespace Roguelike;

public class Pathfinder
{
    // private readonly TileMap _tileMap;

    private Dictionary<TileType, bool> _tilePassable = new();

    private Dictionary<TileType, int> _tileWeights = new();


    public Pathfinder()
    {
        // _tileMap = tm;

        foreach (int i in Enum.GetValues(typeof(TileType)))
        {
            _tileWeights[(TileType)i] = i;
            _tilePassable[(TileType)i] = i < (int)TileType.Wall;
        }
    }

    public void SetTileWeight(TileType type, int weight)
    {
        _tileWeights[type] = weight;
    }
    
    public void SetTilePassable(TileType type, bool passable)
    {
        _tilePassable[type] = passable;
    }
    
    public Stack<DungeonTile> FindPath(TileMap tileMap, IntVector2 start, IntVector2 end)
    {
        var startTile = tileMap.GetTileAt(start);
        var endTile = tileMap.GetTileAt(end);
        

        var path = new Stack<DungeonTile>();
        var openList = new PriorityQueue<DungeonTile,float>();
        var closedList = new List<DungeonTile>();
        List<DungeonTile> adjacencies;
        DungeonTile current = startTile;
       
        // add start node to Open List
        openList.Enqueue(startTile, startTile.F);

        while(openList.Count != 0 && !closedList.Exists(t => t.X == endTile.X && t.Y == endTile.Y))
        {
            current = openList.Dequeue();
            closedList.Add(current);
            adjacencies = tileMap.GetAdjacentTiles(current);

            foreach(DungeonTile adj in adjacencies)
            {
                if (!closedList.Contains(adj) && _tilePassable[adj.Type])
                {
                    bool isFound = false;
                    foreach (var oLNode in openList.UnorderedItems)
                    {
                        if (oLNode.Element == adj)
                        {
                            isFound = true;
                        }
                    }
                    if (!isFound)
                    {
                        adj.Parent = current;
                        adj.DistanceToTarget = Math.Abs(adj.X - end.X) + Math.Abs(adj.Y - end.Y);
                        adj.Cost = _tileWeights[adj.Type] + adj.Parent.Cost;
                        openList.Enqueue(adj, adj.F);
                    }
                }
            }
        }
        
        // construct path, if end was not closed return null
        if(!closedList.Exists(t => t.X == (int)end.X && t.Y == (int)end.Y))
        {
            return null;
        }

        // if all good, return path
        DungeonTile temp = closedList[closedList.IndexOf(current)];
        if (temp == null) return null;
        do
        {
            path.Push(temp);
            temp = temp.Parent;
        } while (temp != null && (temp.X != startTile.X || temp.Y != startTile.Y)) ;
        return path;
    }
    

}
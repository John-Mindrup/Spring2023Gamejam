using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class AStar
{

    public static AStar Instance { get; private set; }

    private const int MOVE_STRAIT_COST = 10;
    private const int MOVE_DIAGONAL_COST = 14;

    public GameObject baseTile;

    List<List<Pathnode>> list;

    private List<Pathnode> openList;
    private List<Pathnode> closedList;

    public int width, height;
    public Tilemap map { get; private set; }

    public AStar(int width, int height, GameObject baseTile, Tilemap obs)
    {
        this.baseTile = baseTile;
        this.width = width;
        this.height = height;
        map = obs;
        list = new List<List<Pathnode>>();
        for(int i = 0; i < width; i++)
        {
            list.Add(new List<Pathnode>());
            for(int j = 0; j < height; j++)
            {
                list[i].Add(new Pathnode(list, i, j, baseTile));
            }
        }
        Instance = this;
    }

    public List<Pathnode> FindPath(int startX, int startY, int endX, int endY, Gloob g)
    {
        Vector3 start = new Vector3(startX, startY);
        Vector3Int gridpos = WorldPosToGridPos(start);
        Vector3 end = new Vector3(endX, endY);
        Vector3Int endgridpos = WorldPosToGridPos(end);
        Pathnode startNode = list[gridpos.x][gridpos.y];
        Pathnode endNode = list[endgridpos.x][endgridpos.y];
        openList = new List<Pathnode> { startNode } ;
        closedList = new List<Pathnode>();

        for(int x = 0; x < width; x++)
        {
            for(int y = 0; y < height; y++)
            {
                Pathnode pathNode = list[x][y];
                pathNode.gCost = int.MaxValue;
                pathNode.calculateFCost();
                pathNode.prev = null;
            }
        }

        startNode.gCost = 0;
        startNode.hCost = CalculateDistanceCost(startNode, endNode);
        startNode.calculateFCost();

        while(openList.Count > 0)
        {
            Pathnode cur = GetLowestFCost(openList);
            if(cur == endNode)
            {
                //reached end
                return CalculatePath(endNode);
            }

            openList.Remove(cur);
            closedList.Add(cur);
            
            foreach(Pathnode neighbor in GetNeighbourList(cur))
            {
                if (closedList.Contains(neighbor)) continue;
                if (!IsPassable(neighbor,g))
                {
                    closedList.Add(neighbor);
                    continue;
                }
                int tempGCost = cur.gCost + CalculateDistanceCost(cur, neighbor);
                if(tempGCost < neighbor.gCost)
                {
                    neighbor.prev = cur;
                    neighbor.gCost = tempGCost;
                    neighbor.hCost = CalculateDistanceCost(cur, neighbor);
                    neighbor.calculateFCost();

                    if (!openList.Contains(neighbor))
                    {
                        openList.Add(neighbor);
                    }
                }
            }

        }
        //out of nodes
        return null;
    }

    private List<Pathnode> CalculatePath(Pathnode node)
    {

        List<Pathnode> path = new List<Pathnode>();
        path.Add(node);
        Pathnode cur = node;
        while(cur.prev != null)
        {
            path.Add(cur.prev);
            cur = cur.prev;
        }
        path.Reverse();
        return path;
    }

    private bool IsPassable(Pathnode p, Gloob g)
    {
        Vector3Int locationofdest = Vector3Int.FloorToInt(p.worldpos);
        Tile t = map.GetTile<Tile>(locationofdest);
        if (t != null)
        {
            if (t == SceneStats.Instance.hole && g.weight <= 2)
                return true;
            if (t == SceneStats.Instance.fire && g.fireProof)
                return true;
            if (t == SceneStats.Instance.pit && g.hover)
                return true;
            return false;
        }
        return true;
    }

    private int CalculateDistanceCost(Pathnode a, Pathnode b)
    {
        
        int xDist = Mathf.Abs(a.x - b.x);
        int yDist = Mathf.Abs(a.y - b.y);
        int remaining = Mathf.Abs(xDist - yDist);
        return MOVE_DIAGONAL_COST * Mathf.Min(xDist, yDist) + MOVE_STRAIT_COST * remaining;
    }

    private Pathnode GetLowestFCost(List<Pathnode> l)
    {
        Pathnode ret = l[0];
        for (int i = 1; i < l.Count; i++)
        {
            if (l[i].fCost < ret.fCost)
            {
                ret = l[i];
            }
        }
        return ret;
    }

    private List<Pathnode> GetNeighbourList(Pathnode currentNode)
    {
        List<Pathnode> neighbourList = new List<Pathnode>();

        if (currentNode.x - 1 >= 0)
        {
            // Left
            neighbourList.Add(list[currentNode.x - 1] [currentNode.y]);
            // Left Down
            if (currentNode.y - 1 >= 0) neighbourList.Add(list[currentNode.x - 1][currentNode.y - 1]);
            // Left Up
            if (currentNode.y + 1 < height) neighbourList.Add(list[currentNode.x - 1][currentNode.y + 1]);
        }
        if (currentNode.x + 1 < width)
        {
            // Right
            neighbourList.Add(list[currentNode.x + 1][currentNode.y]);
            // Right Down
            if (currentNode.y - 1 >= 0) neighbourList.Add(list[currentNode.x + 1][currentNode.y - 1]);
            // Right Up
            if (currentNode.y + 1 < height) neighbourList.Add(list[currentNode.x + 1][currentNode.y + 1]);
        }
        // Down
        if (currentNode.y - 1 >= 0) neighbourList.Add(list[currentNode.x][currentNode.y - 1]);
        // Up
        if (currentNode.y + 1 < height) neighbourList.Add(list[currentNode.x][currentNode.y + 1]);

        return neighbourList;
    }

    private Vector3Int WorldPosToGridPos(Vector3 pos)
    {
        Vector3Int ret = Vector3Int.FloorToInt(pos - baseTile.transform.position);
        return ret;
    }


}

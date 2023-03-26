using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Pathnode
{
    private List<List<Pathnode>> grid;
    public int x;
    public int y;
    public Vector3 worldpos;

    public int gCost;
    public int hCost;
    public int fCost;

    Tile tile;
    public Pathnode prev;

    public Pathnode(List<List<Pathnode>> grid, int x, int y, GameObject baseTile)
    {
        this.grid = grid;
        this.x = x;
        this.y = y;
        worldpos= new Vector3(baseTile.transform.position.x + x, baseTile.transform.position.y +y);
    }

    public void calculateFCost()
    {
        fCost = gCost + hCost;
    }
}

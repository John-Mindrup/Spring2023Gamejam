using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SceneStats : MonoBehaviour
{
    public int width, height;
    public GameObject baseTile;
    public Tile hole, pit, fire;
    private static Tile Gate;
    public Tilemap obstacles;
    public Vector3Int gateCords;
    public int locks;

    public static SceneStats Instance { get; private set; }

    private void Start()
    {
        AStar a = new AStar(width, height, baseTile, obstacles);
        Gate = obstacles.GetTile<Tile>(gateCords);
        Instance = this;
    }

    private void FixedUpdate()
    {
        if (locks == 0) { OpenGate(); }
        if (locks > 0) { CloseGate(); }
    }

    public void OpenGate()
    {
        Instance.obstacles.SetTile(Instance.gateCords, null);
    }
    public void CloseGate()
    {
        Instance.obstacles.SetTile(Instance.gateCords, Gate);
    }
}

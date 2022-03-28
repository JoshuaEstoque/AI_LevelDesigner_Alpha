using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class AstarDebug : MonoBehaviour
{
    [SerializeField]
    private Grid grid;

    [SerializeField]
    private Tilemap tilemap;

    [SerializeField]
    private Tile tile;

    [SerializeField]
    private Color openColor, closedColor, pathColor, currentColor, startColor, goalColor;

    [SerializeField]
    private Canvas canvas;

    [SerializeField]
    private GameObject debugTextPrefab;

    public int tileCount = 0;

    public Tilemap AstarMap;

    private List<GameObject> debugObjects = new List<GameObject>();

    public int CreateTile(HashSet<Node> openList, HashSet<Node> closedList, Vector3Int start, Vector3Int goal, Tilemap tilemap, Stack<Vector3Int> path = null)
    {
        //Debug.Log("HELP");
        foreach (Node node in openList)
        {
            ColorTile(node.Position, openColor, tilemap);
        }

        foreach (Node node in closedList)
        {
            ColorTile(node.Position, closedColor, tilemap);
        }
        tileCount = 0;
        if (path != null)
        {
            foreach (Vector3Int pos in path)
            {
                if(pos!=start && pos!= goal)
                {
                    tileCount++;
                    ColorTile(pos, pathColor, tilemap);
                }
            }
        }

        ColorTile(start, startColor, tilemap);
        ColorTile(goal, goalColor, tilemap);
        return tileCount;
        //Debug.Log("DONE!_____________________________________");
    }

    public void ColorTile(Vector3Int position, Color color, Tilemap tilemap2)
    {
        //Debug.Log("Colored tile at: "+position);
        //tilemap2.SetTile(position, tile);
        //tilemap2.SetTileFlags(position, TileFlags.None);
        //tilemap2.SetColor(position, color);
    }


    public void ClearMap()
    {
        AstarMap.ClearAllTiles();
    }
}

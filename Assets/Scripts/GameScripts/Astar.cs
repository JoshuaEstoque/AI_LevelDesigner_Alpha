using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;
using System;


public enum TileType { TILE, SPIKE, GOAL, EMPTY }

public class Astar : MonoBehaviour
{
    private int DebugCount = 0;
    public Transform spawnPoint;
    public Transform goalPoint;

    [SerializeField]
    private Tilemap AstarMap;

    [SerializeField]
    private Tilemap tilemap;

    [SerializeField]
    private Tile solidTile;

    [SerializeField]
    public GameObject my_AstarDebug;

    AstarDebug myAstarDebug_script;

    private HashSet<Node> openList;
    private HashSet<Node> closedList;
    private Node current;
    private Dictionary<Vector3Int, Node> allNodes = new Dictionary<Vector3Int, Node>();

    private Stack<Vector3Int> path;

    public bool hasPath = false;

    private Vector3 startpos, endpos;

    [SerializeField]
    private int AstarDistance;



    void Awake()
    {
    }

    void Start()
    {
        myAstarDebug_script = my_AstarDebug.GetComponent<AstarDebug>();
    }
    // Update is called once per frame
    void Update()
    {
    }

    public void jumpMapping()
    {
    }

    //Main Function
    public int Algorithm(Vector3 startpos, Vector3 endpos)
    {
        /*
        startpos = spawnPoint.localPosition;
        endpos = goalPoint.localPosition;
        startpos = new Vector3Int((int)startpos.x, (int)startpos.y, (int)startpos.z);
        endpos = new Vector3Int((int)endpos.x, (int)endpos.y, (int)endpos.z);
        */
        startpos = spawnPoint.localPosition;
        endpos = goalPoint.localPosition;

        startpos = Vector3Int.FloorToInt(startpos);
        endpos = Vector3Int.FloorToInt(endpos);

        startpos = new Vector3Int((int)startpos.x, (int)startpos.y, (int)startpos.z);
        endpos = new Vector3Int((int)endpos.x, (int)endpos.y, (int)endpos.z);

        //Debug.Log("The end position is " + endpos);
        myAstarDebug_script.ClearMap();

        if (current == null)
        {
            Initialize(startpos);
        }
        else
        {
            current = null;
            path = null;
            Initialize(startpos);
        }

        if (IsTileOfType<Tile>(tilemap, Vector3Int.FloorToInt(startpos)))
        {
            hasPath = false;
            return 1000;
        }

        if (IsTileOfType<Tile>(tilemap, Vector3Int.FloorToInt(endpos)))
        {
            hasPath = false;
            return 1000;
        }

        while (openList.Count > 0 && path == null)
        {
            List<Node> neighbours = FindNeighbours(current.Position, startpos);

            ExamineNeighbours(neighbours, current, endpos);

            UpdateCurrentTile(ref current);

            path = GeneratePath(current, startpos, endpos);
        }

        AstarDistance = myAstarDebug_script.CreateTile(openList, closedList, Vector3Int.FloorToInt(startpos), Vector3Int.FloorToInt(endpos), AstarMap, path);

        if (path == null)
        {
            //Debug.Log("NO PATH FOUND!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");

            Initialize(startpos);
            return 1000;
        }
        else
        {
            //Debug.Log("PATH FOUND!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
            hasPath = true;

            Initialize(startpos);
        }
        return AstarDistance;
    }

    private List<Node> FindNeighbours(Vector3Int parentPosition, Vector3 startpos)
    {
        List<Node> neighbours = new List<Node>();
        Vector3Int neighbourPos;
        Node neighbour;

        //check at y-1
        neighbourPos = new Vector3Int(parentPosition.x, parentPosition.y - 1, parentPosition.z);
        if (neighbourPos != startpos && tilemap.GetTile(neighbourPos) && IsTileOfType<Tile>(tilemap, neighbourPos) == false)
        {
            neighbour = GetNode(neighbourPos);
            neighbours.Add(neighbour);
        }

        //check at y+1
        neighbourPos = new Vector3Int(parentPosition.x, parentPosition.y + 1, parentPosition.z);
        if (neighbourPos != startpos && tilemap.GetTile(neighbourPos) && IsTileOfType<Tile>(tilemap, neighbourPos) == false)
        {
            neighbour = GetNode(neighbourPos);
            neighbours.Add(neighbour);
        }

        //check at x-1
        neighbourPos = new Vector3Int(parentPosition.x - 1, parentPosition.y, parentPosition.z);
        if (neighbourPos != startpos && tilemap.GetTile(neighbourPos) && IsTileOfType<Tile>(tilemap, neighbourPos) == false)
        {
            neighbour = GetNode(neighbourPos);
            neighbours.Add(neighbour);
        }

        //check at x+1
        neighbourPos = new Vector3Int(parentPosition.x + 1, parentPosition.y, parentPosition.z);
        if (neighbourPos != startpos && tilemap.GetTile(neighbourPos) && IsTileOfType<Tile>(tilemap, neighbourPos) == false)
        {
            neighbour = GetNode(neighbourPos);
            neighbours.Add(neighbour);
        }

        return neighbours;

    }

    private void ExamineNeighbours(List<Node> neighbours, Node current, Vector3 endpos)
    {
        for (int i = 0; i < neighbours.Count; i++)
        {
            Node neighbour = neighbours[i];
            int gScore = DetermineGScore(neighbours[i].Position, (current.Position));

            if (openList.Contains(neighbour))
            {
                if (current.G + gScore < neighbour.G)
                {
                    CalcValues(current, neighbour, gScore, endpos);
                }
            }
            else if (!closedList.Contains(neighbour))
            {
                CalcValues(current, neighbour, gScore, endpos);

                openList.Add(neighbour);
            }
        }
    }

    private void CalcValues(Node parent, Node neighbour, int cost, Vector3 endpos)
    {
        neighbour.Parent = parent;

        neighbour.G = parent.G + cost;
        neighbour.H = (Math.Abs(neighbour.Position.x - (int)endpos.x) + Math.Abs(neighbour.Position.y - (int)endpos.y));
        neighbour.F = neighbour.G + neighbour.H;

    }

    private int DetermineGScore(Vector3Int neighbour, Vector3Int current)
    {
        int gScore = 0;

        int x = current.x - neighbour.x;
        int y = current.x - neighbour.y;

        if (Math.Abs(x - y) % 2 == 1)
        {
            gScore = 1;
        }
        else
        {
            gScore = 1;
        }
        return gScore;
    }

    private void UpdateCurrentTile(ref Node current)
    {
        openList.Remove(current);

        closedList.Add(current);

        if (openList.Count > 0)
        {
            current = openList.OrderBy(x => x.F).First();
        }
    }

    private void Initialize(Vector3 startpos)
    {
        //Debug.Log("<><><><><><>INITIALIZING<><><><><><>");
        current = GetNode(new Vector3Int((int)startpos.x, (int)startpos.y, (int)startpos.z));

        openList = new HashSet<Node>();
        closedList = new HashSet<Node>();

        //Adding start point to open list
        openList.Add(current);
        //Debug.Log("The current position is " + current.Position);
        //Debug.Log("The start position is " + startpos);

    }

    private Node GetNode(Vector3Int position)
    {
        if (allNodes.ContainsKey(position))
        {
            return allNodes[position];
        }
        else
        {
            Node node = new Node(position);
            allNodes.Add(position, node);
            return node;
        }
    }

    private Stack<Vector3Int> GeneratePath(Node current, Vector3 startpos, Vector3 endpos)
    {
        if (current.Position == endpos)
        {
            Stack<Vector3Int> finalpath = new Stack<Vector3Int>();
            while (current.Position != startpos)
            {
                finalpath.Push(current.Position);

                current = current.Parent;
            }
            return finalpath;
        }
        return null;
    }

    public bool IsTileOfType<Tile>(Tilemap tilemap, Vector3Int position) where Tile : TileBase
    {
        TileBase targetTile = tilemap.GetTile(position);

        if (targetTile == solidTile)
        {
            return true;
        }

        return false;
    }



}

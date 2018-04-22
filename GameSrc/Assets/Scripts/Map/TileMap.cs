using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TileMap : MonoBehaviour
{
    public static TileMap Instance { get; private set; }

    public void Awake()
    {
        Instance = this;
    }

    public int width = 15;
    public int height = 11;
    public float gap = 0.1f;
    float hexWidth = 1.732f;
    float hexHeight = 2.0f;
    Vector3 startPos;

    public TileType[] tileTypes;
    Node[,] graph;
    List<Node> currentPath = null;
    int[,] tiles;

    void Start()
    {
        CreateTile();
        CalcStartPos();
        CreateTileMap();
        GeneratePathfindingGraph();
    }

    void CalcStartPos()
    {
        float offset = 0;
        if (height / 2 % 2 != 0)
            offset = hexWidth / 2;

        float x = -hexWidth * (width / 2) - offset;
        float z = hexHeight * 0.75f * (height / 2);

        startPos = new Vector3(x, 0, z);
    }

    public Vector3 CalcWorldPos(Vector2 tilePos)
    {
        float offset = 0;
        if (tilePos.y % 2 != 0)
            offset = hexWidth / 2;

        float x = startPos.x + tilePos.x * hexWidth + offset;
        float z = startPos.z - tilePos.y * hexHeight * 0.75f;

        return new Vector3(x, 0, z);
    }

    public Vector2 GetTileCoordinatesByWorldPosition(Vector3 tileWorldPos)
    {
        float offset = 0;
        if ((int)tileWorldPos.x % 2 != 0)
            offset = hexWidth / 2;

        float x = (tileWorldPos.x - startPos.x - offset) / hexWidth;
        float z = (tileWorldPos.z - startPos.z) / (hexHeight * 0.75f) * (-1.0f);

        return new Vector2((int)x, (int)z);
    }

    void CreateTile()
    {
        hexWidth += hexWidth * gap;
        hexHeight += hexHeight * gap;
    }

    void CreateTileMap()
    {
        const int swampLimit = 7;
        const int mountainLimit = 4;
        int countSwampTile = 0;
        int countMountainTile = 0;
        System.Random rand = new System.Random();
        tiles = new int[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                #region get tile type
                TileType tt;
                int tileType = 0;
                if (x > width / 3)
                {
                    tileType = rand.Next(3);
                }

                if (tileType == 1 && countMountainTile != mountainLimit)
                {
                    tt = tileTypes[1];
                    ++countMountainTile;
                }
                else if (tileType == 2 && countSwampTile != swampLimit)
                {
                    tt = tileTypes[2];
                    ++countSwampTile;
                }
                else
                {
                    tt = tileTypes[0];
                }
                #endregion

                Tile currentTile = new GameObject().AddComponent<Tile>();
                Vector2 tilePos = new Vector2(x, y);
                currentTile.Create(tt.tileVisualPrefab.transform, tilePos);
                tiles[x, y] = TileType.GetTileTypeByTileName(tt.name);
            }
        }
    }

    void GeneratePathfindingGraph()
    {
        graph = new Node[width, height];

        // Initialize a Node for each spot in the array
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                graph[x, y] = new Node();
                graph[x, y].x = x;
                graph[x, y].y = y;
            }
        }

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                InsertNeighbours(x, y, ref graph);
            }
        }
    }

    void InsertNeighbours(int x, int y, ref Node[,] graph)
    {
        //even
        if (y % 2 == 0)
        {
            if (x > 0 && x < width - 1)
            {
                graph[x, y].neighbours.Add(graph[x - 1, y]);
                graph[x, y].neighbours.Add(graph[x + 1, y]);
            }
            if (y > 0 && x > 0)
            {
                graph[x, y].neighbours.Add(graph[x - 1, y - 1]);
                graph[x, y].neighbours.Add(graph[x, y - 1]);
            }
            if (y < height - 1 && x > 0)
            {
                graph[x, y].neighbours.Add(graph[x - 1, y + 1]);
                graph[x, y].neighbours.Add(graph[x, y + 1]);
            }

            //borders
            if (x == 0 && y != 0 && y != height - 1)
            {
                graph[x, y].neighbours.Add(graph[x, y - 1]);
                graph[x, y].neighbours.Add(graph[x + 1, y]);
                graph[x, y].neighbours.Add(graph[x, y + 1]);
            }
            else if (x == width - 1 && y != 0 && y != height - 1)
            {
                graph[x, y].neighbours.Add(graph[x - 1, y]);
            }
            else if (x == 0 && y == 0)
            {
                graph[x, y].neighbours.Add(graph[x + 1, y]);
                graph[x, y].neighbours.Add(graph[x, y + 1]);
            }
            else if (x == 0 && y == height - 1)
            {
                graph[x, y].neighbours.Add(graph[x + 1, y]);
                graph[x, y].neighbours.Add(graph[x, y - 1]);
            }
            else if (x == width - 1 && y == 0)
            {
                graph[x, y].neighbours.Add(graph[x - 1, y]);
            }
            else if (x == width - 1 && y == height - 1)
            {
                graph[x, y].neighbours.Add(graph[x - 1, y]);
            }
        }
        //odd
        else
        {
            if (x > 0 && x < width - 1)
            {
                graph[x, y].neighbours.Add(graph[x - 1, y]);
                graph[x, y].neighbours.Add(graph[x + 1, y]);
            }
            if (y > 0 && x < width - 1)
            {
                graph[x, y].neighbours.Add(graph[x, y - 1]);
                graph[x, y].neighbours.Add(graph[x + 1, y - 1]);
            }
            if (y < height - 1 && x < width - 1)
            {
                graph[x, y].neighbours.Add(graph[x, y + 1]);                
                graph[x, y].neighbours.Add(graph[x + 1, y + 1]);
            }

            //borders
            if (x == 0)
            {
                graph[x, y].neighbours.Add(graph[x + 1, y]);
            }
            else if (x == width - 1)
            {
                graph[x, y].neighbours.Add(graph[x, y - 1]);
                graph[x, y].neighbours.Add(graph[x - 1, y]);
                graph[x, y].neighbours.Add(graph[x, y + 1]);
            }
        }
    }

    //---------------------------------UNIT---------------------------------------------

    public Unit[] playerUnits;
    static public Unit selectedUnit;
    static Color startPlayerColor;
    static Color selectedPlayerColor = new Color(0.1f, 0.1f, 0.9f);

    public void SelectUnit(Unit unit)
    {
        selectedUnit = unit;
        startPlayerColor = selectedUnit.GetComponent<Renderer>().material.GetColor("_Color");
        selectedUnit.GetComponent<Renderer>().material.SetColor("_Color", selectedPlayerColor);
    }
    public void DeselectUnit()
    {
        if (selectedUnit != null)
        {
            selectedUnit.GetComponent<Renderer>().material.SetColor("_Color", startPlayerColor);
            selectedUnit = null;
        }
    }

    public void GeneratePathTo(float x, float z)
    {
        selectedUnit.currentPath = null;
        Dictionary<Node, float> dist = new Dictionary<Node, float>();
        Dictionary<Node, Node> prev = new Dictionary<Node, Node>();
        List<Node> unvisited = new List<Node>();

        Vector2 sourceLocalPos = GetTileCoordinatesByWorldPosition(new Vector3(selectedUnit.tileX, 0, selectedUnit.tileZ));
        Vector2 targetLocalPos = GetTileCoordinatesByWorldPosition(new Vector3(x, 0, z));

        Node source = graph[(int)sourceLocalPos.x, (int)sourceLocalPos.y];
        Node target = graph[(int)targetLocalPos.x, (int)targetLocalPos.y];


        dist[source] = 0;
        prev[source] = null;

        foreach (var vertex in graph)
        {
            if (vertex != source)
            {
                dist[vertex] = Mathf.Infinity;
                prev[vertex] = null;
            }
            unvisited.Add(vertex);
        }

        while (unvisited.Count > 0)
        {
            Node u = null;

            foreach (var possibleU in unvisited)
            {
                if (u == null || dist[u] > dist[possibleU])
                {
                    u = possibleU;
                }
            }

            if (u == target)
            {
                break;
            }

            unvisited.Remove(u);

            foreach (var v in u.neighbours)
            {
                //float alt = dist[u] + u.DistanceTo(v); //without calc tile cost
                float alt = dist[u] + CostToEnterTile(v.x, v.y);
                if (alt < dist[v])
                {
                    dist[v] = alt;
                    prev[v] = u;
                }
            }
        }

        if (prev[target] == null)
        {
            //No route between source and target
            return;
        }
        else
        {
            currentPath = new List<Node>();
            Node currentTile = target;

            while (currentTile != null)
            {
                currentPath.Add(currentTile);
                currentTile = prev[currentTile];
            }

            currentPath.Reverse();
            selectedUnit.currentPath = currentPath;
        }
    }

    public void MoveSelectedUnitTo(float x, float z)
    {
        if (selectedUnit != null && isEmptyTile(x, z))
        {
            //selectedUnit.transform.position = new Vector3(x, selectedUnit.transform.position.y, z);  
            
            selectedUnit.UpdatePosition(x, z);
            DeselectUnit();
        }
    }

    private bool isEmptyTile(float x, float z)
    {
        foreach (var item in playerUnits)
        {
            if ((int)item.tileX == (int)x && (int)item.tileZ == (int)z)
            {
                if (item.tileX * x > 0)
                {
                    return false;
                }
            }
        }

        return true;
    }    

    public List<Node> getNeighboursByUnitPos(Vector2 unitPos)
    {
        return graph[(int)unitPos.x, (int)unitPos.y].neighbours;
    }

    int CostToEnterTile(int x, int y)
    {
        return tileTypes[tiles[x, y]].movementCost;
    }
}

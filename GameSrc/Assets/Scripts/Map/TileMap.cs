using System;
using System.Collections.Generic;
using UnityEngine;

public class TileMap : MonoBehaviour
{
    public Unit[] playerUnits;

    static private Unit selectedUnit;
    static Color startPlayerColor;
    static Color selectedPlayerColor = new Color(0.1f,0.1f,0.9f);

    public void resetSelectedUnit()
    {
        if (selectedUnit != null)
        {
            selectedUnit.GetComponent<Renderer>().material.SetColor("_Color", startPlayerColor);
            selectedUnit = null;
        }
    }

    public void setSelectedUnit(Unit unit)
    {
        selectedUnit = unit;
        startPlayerColor = selectedUnit.GetComponent<Renderer>().material.GetColor("_Color");
        selectedUnit.GetComponent<Renderer>().material.SetColor("_Color", selectedPlayerColor);
        //selectTiles(unit);
    }

    private void selectTiles(Unit unit)
    {
        //float x = unit.transform.position.x;
        //float y = unit.transform.position.y;
        //float z = unit.transform.position.z;
        //Node[,] _graph = graph;

        //Vector3 worldPos = CalcWorldPos(new Vector2(x, z));
    }

    class Node
    {
        public List<Node> neighbours;
        public Node()
        {
            neighbours = new List<Node>();
        }
    }

    public TileType[] tileTypes;
    Vector3 startPos;

    public int tileWidth = 15;
    public int tileHeight = 11;
    public float gap = 0.1f;

    float hexWidth = 1.732f;
    float hexHeight = 2.0f;

    Node[,] graph;

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
        if (tileHeight / 2 % 2 != 0)
            offset = hexWidth / 2;

        float x = -hexWidth * (tileWidth / 2) - offset;
        float z = hexHeight * 0.75f * (tileHeight / 2);

        startPos = new Vector3(x, 0, z);
    }

    Vector3 CalcWorldPos(Vector2 tilePos)
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

        int x = (int)((tileWorldPos.x - startPos.x - offset) / hexWidth);
        float z = ((tileWorldPos.z - startPos.z) / (hexHeight * 0.75f) * (-1.0f));

        return new Vector2(x, (int)z);
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

        for (int x = 0; x < tileWidth; x++)
        {
            for (int y = 0; y < tileHeight; y++)
            {
                TileType tt;
                int tileType = 0;
                if (x > tileWidth / 3)
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

                Transform hex = Instantiate(tt.tileVisualPrefab.transform) as Transform;
                Vector2 tilePos = new Vector2(x, y);

                hex.position = CalcWorldPos(tilePos);
                hex.parent = this.transform;
                hex.name = "Hexagon" + x + "|" + y;

                ClickableTile ct;
                if (hex.GetComponent<ClickableTile>() != null)
                {
                    ct = hex.GetComponent<ClickableTile>();
                    ct.tileX = hex.position.x;
                    ct.tileY = hex.position.z;
                    ct.map = this;
                }
            }
        }
    }

    void GeneratePathfindingGraph()
    {
        graph = new Node[tileWidth, tileHeight];

        // Initialize a Node for each spot in the array
        for (int x = 0; x < tileWidth; x++)
        {
            for (int y = 0; y < tileHeight; y++)
            {
                graph[x, y] = new Node();
            }
        }

        for (int x = 0; x < tileWidth; x++)
        {
            for (int y = 0; y < tileHeight; y++)
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
            if (x > 0 && x < tileWidth - 1)
            {
                graph[x, y].neighbours.Add(graph[x - 1, y]);
                graph[x, y].neighbours.Add(graph[x + 1, y]);
            }
            if (y > 0 && x > 0)
            {
                graph[x, y].neighbours.Add(graph[x - 1, y - 1]);
                graph[x, y].neighbours.Add(graph[x, y - 1]);
            }
            if (y < tileHeight - 1 && x > 0)
            {
                graph[x, y].neighbours.Add(graph[x - 1, y + 1]);
                graph[x, y].neighbours.Add(graph[x, y + 1]);
            }

            //borders
            if (x == 0 && y != 0 && y != tileHeight - 1)
            {
                graph[x, y].neighbours.Add(graph[x, y - 1]);
                graph[x, y].neighbours.Add(graph[x + 1, y]);
                graph[x, y].neighbours.Add(graph[x, y + 1]);
            }
            else if (x == tileWidth - 1 && y != 0 && y != tileHeight - 1)
            {
                graph[x, y].neighbours.Add(graph[x - 1, y]);
            }
            else if (x == 0 && y == 0)
            {
                graph[x, y].neighbours.Add(graph[x + 1, y]);
                graph[x, y].neighbours.Add(graph[x, y + 1]);
            }
            else if (x == 0 && y == tileHeight - 1)
            {
                graph[x, y].neighbours.Add(graph[x + 1, y]);
                graph[x, y].neighbours.Add(graph[x, y - 1]);
            }
            else if (x == tileWidth - 1 && y == 0)
            {
                graph[x, y].neighbours.Add(graph[x - 1, y]);
            }
            else if (x == tileWidth - 1 && y == tileHeight - 1)
            {
                graph[x, y].neighbours.Add(graph[x - 1, y]);
            }
        }
        //odd
        else
        {
            if (x > 0 && x < tileWidth - 1)
            {
                graph[x, y].neighbours.Add(graph[x - 1, y]);
                graph[x, y].neighbours.Add(graph[x + 1, y]);
            }
            if (y > 0 && x < tileWidth - 1)
            {
                graph[x, y].neighbours.Add(graph[x, y - 1]);
                graph[x, y].neighbours.Add(graph[x + 1, y - 1]);
            }
            if (y < tileHeight - 1 && x < tileWidth - 1)
            {
                graph[x, y].neighbours.Add(graph[x, y + 1]);                
                graph[x, y].neighbours.Add(graph[x + 1, y + 1]);
            }

            //borders
            if (x == 0)
            {
                graph[x, y].neighbours.Add(graph[x + 1, y]);
            }
            else if (x == tileWidth - 1)
            {
                graph[x, y].neighbours.Add(graph[x, y - 1]);
                graph[x, y].neighbours.Add(graph[x - 1, y]);
                graph[x, y].neighbours.Add(graph[x, y + 1]);
            }
        }
    }

    public void MoveSelectedUnitTo(float x, float z)
    {
        if (selectedUnit != null && isEmptyTile(x, z))
        {
            selectedUnit.transform.position = new Vector3(x, selectedUnit.transform.position.y, z);
            selectedUnit.UpdatePosition(x, z);
            resetSelectedUnit();
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

    //public TileMap getTileMap()
    //{
    //    Unit unit = selectedUnit.GetComponent<Unit>();
    //    unit.map = this;
    //    return unit.map;
    //}
}

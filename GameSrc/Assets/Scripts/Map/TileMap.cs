using System;
using System.Collections.Generic;
using UnityEngine;

public class TileMap : MonoBehaviour
{
    public GameObject selectedUnit;

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
                graph[x, y].neighbours.Add(graph[x, y - 1]);
                graph[x, y].neighbours.Add(graph[x - 1, y - 1]);
                graph[x, y].neighbours.Add(graph[x - 1, y]);
                graph[x, y].neighbours.Add(graph[x - 1, y + 1]);
                graph[x, y].neighbours.Add(graph[x, y + 1]);
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
                graph[x, y].neighbours.Add(graph[x - 1, y + 1]);
                graph[x, y].neighbours.Add(graph[x, y + 1]);
            }
            else if (x == tileWidth - 1 && y == tileHeight - 1)
            {
                graph[x, y].neighbours.Add(graph[x - 1, y]);
                graph[x, y].neighbours.Add(graph[x - 1, y - 1]);
                graph[x, y].neighbours.Add(graph[x, y - 1]);
            }
            else if (y == 0)
            {
                graph[x, y].neighbours.Add(graph[x - 1, y]);
                graph[x, y].neighbours.Add(graph[x - 1, y + 1]);
                graph[x, y].neighbours.Add(graph[x, y + 1]);
                graph[x, y].neighbours.Add(graph[x + 1, y]);
            }
            else if (y == tileHeight - 1)
            {
                graph[x, y].neighbours.Add(graph[x - 1, y]);
                graph[x, y].neighbours.Add(graph[x - 1, y - 1]);
                graph[x, y].neighbours.Add(graph[x, y - 1]);
                graph[x, y].neighbours.Add(graph[x + 1, y]);
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
                graph[x, y].neighbours.Add(graph[x, y - 1]);
                graph[x, y].neighbours.Add(graph[x + 1, y - 1]);
                graph[x, y].neighbours.Add(graph[x + 1, y]);
                graph[x, y].neighbours.Add(graph[x + 1, y + 1]);
                graph[x, y].neighbours.Add(graph[x, y + 1]);
            }
            else if (x == tileWidth - 1)
            {
                graph[x, y].neighbours.Add(graph[x, y - 1]);
                graph[x, y].neighbours.Add(graph[x - 1, y]);
                graph[x, y].neighbours.Add(graph[x, y + 1]);
            }
        }
    }

    public void MoveSelectedUnitTo(float x, float y)
    {
        selectedUnit.transform.position = new Vector3(x, 0.6f, y);
    }
}

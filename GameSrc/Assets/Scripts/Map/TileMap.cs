using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TileMap : MonoBehaviour
{
    public int width = 11;
    public int height = 9;
    public float gap = 0.1f;
    public static float hexWidth = 1.732f;
    public static float hexHeight = 2.0f;
    public static Vector3 startPos;

    public TileType[] tileTypes;
    public Node[,] graph;    
    public int[,] tiles;

    void Start()
    {
        Debug.Log("TileMapStart");
        AddGap();
        CalcStartPos();
        CreateTileMap();
        GeneratePathfindingGraph();
    }

    void AddGap()
    {
        hexWidth += hexWidth * gap;
        hexHeight += hexHeight * gap;
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

    void CreateTileMap()
    {
        const int mountainLimit = 6;
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
                    tileType = rand.Next(2);
                }

                if (tileType == 1 && countMountainTile != mountainLimit)
                {
                    tt = tileTypes[1];
                    ++countMountainTile;
                }
                else
                {
                    tt = tileTypes[0];
                }
                #endregion
                
                Vector2 tilePos = new Vector2(x, y);

                Transform tile = Instantiate(tt.tileVisualPrefab.transform) as Transform;

                tile.position = PositionConverter.ToWorldCoordinates(tilePos);
                tile.parent = transform;
                tile.name = "Hexagon" + tilePos.x + "|" + tilePos.y;

                ClickableAvailableTile ct;
                if (tile.GetComponent<ClickableAvailableTile>() != null)
                {
                    ct = tile.GetComponent<ClickableAvailableTile>();
                    ct.tileX = tile.position.x;
                    ct.tileY = tile.position.z;
                }

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
 
    public int CostToEnterTile(int x, int y)
    {
        return tileTypes[tiles[x, y]].movementCost;
    }
}

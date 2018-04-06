using System;
using UnityEngine;

public class TileMap : MonoBehaviour
{
    public TileType[] tileTypes;
    Vector3 startPos;
    Tile tile;

    void Start()
    {
        CreateTile();
        CalcStartPos();
        CreateTileMap();
    }

    void CalcStartPos()
    {
        float offset = 0;
        if (tile.tileHeight / 2 % 2 != 0)
            offset = tile.hexWidth / 2;

        float x = -tile.hexWidth * (tile.tileWidth / 2) - offset;
        float z = tile.hexHeight * 0.75f * (tile.tileHeight / 2);

        startPos = new Vector3(x, 0, z);
    }

    Vector3 CalcWorldPos(Vector2 tilePos)
    {
        float offset = 0;
        if (tilePos.y % 2 != 0)
            offset = tile.hexWidth / 2;

        float x = startPos.x + tilePos.x * tile.hexWidth + offset;
        float z = startPos.z - tilePos.y * tile.hexHeight * 0.75f;

        return new Vector3(x, 0, z);
    }

    void CreateTile()
    {
        tile = new GameObject().AddComponent<Tile>();
        tile.hexWidth += tile.hexWidth * tile.gap;
        tile.hexHeight += tile.hexHeight * tile.gap;
    }

    void CreateTileMap()
    {
        const int swampLimit = 7;
        const int mountainLimit = 4;
        int countSwampTile = 0;
        int countMountainTile = 0;
        System.Random rand = new System.Random();

        for (int y = 0; y < tile.tileHeight; y++)
        {
            for (int x = 0; x < tile.tileWidth; x++)
            {
                TileType tt;
                int tileType = 0;
                if (y > tile.tileWidth / 3)
                {
                    tileType = rand.Next(3);
                }
                
                if (tileType == 1 && countMountainTile != mountainLimit) {
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
            }
        }
    }
}
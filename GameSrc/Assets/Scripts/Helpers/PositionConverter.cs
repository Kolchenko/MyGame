using System;
using UnityEngine;

class PositionConverter
{
    public static Vector3 ToWorldCoordinates(Vector2 tilePos)
    {
        float offset = 0;
        if (tilePos.y % 2 != 0)
            offset = TileMap.hexWidth / 2;

        float x = TileMap.startPos.x + tilePos.x * TileMap.hexWidth + offset;
        float z = TileMap.startPos.z - tilePos.y * TileMap.hexHeight * 0.75f;

        return new Vector3(x, 0, z);
    }

    public static Vector2 ToLocalCoordinates(Vector3 tilePos)
    {
        float offset = 0;
        if ((int)tilePos.x % 2 != 0)
            offset = TileMap.hexWidth / 2;

        float x = (float)((Math.Round(tilePos.x) - TileMap.startPos.x - offset) / TileMap.hexWidth);
        float z = (tilePos.z - TileMap.startPos.z) / (TileMap.hexHeight * 0.75f) * (-1.0f);

        return new Vector2((int)Math.Round(x), (int)Math.Round(z));
    }
}

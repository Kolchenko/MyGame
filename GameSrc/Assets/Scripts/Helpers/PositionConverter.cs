using System;
using UnityEngine;

class PositionConverter
{
    public static WorldPosition ToWorldCoordinates(LocalPosition localPosition)
    {
        float offset = 0;
        if (localPosition.y % 2 != 0)
            offset = TileMap.hexWidth / 2;

        float x = TileMap.startPos.x + localPosition.x * TileMap.hexWidth + offset;
        float z = TileMap.startPos.z - localPosition.y * TileMap.hexHeight * 0.75f;

        return new WorldPosition(x, 0, z);
    }

    public static WorldPosition ToWorldCoordinates(Node node)
    {
        float offset = 0;
        if (node.y % 2 != 0)
            offset = TileMap.hexWidth / 2;

        float x = TileMap.startPos.x + node.x * TileMap.hexWidth + offset;
        float z = TileMap.startPos.z - node.y * TileMap.hexHeight * 0.75f;

        return new WorldPosition(x, 0, z);
    }

    public static LocalPosition ToLocalCoordinates(WorldPosition worldPosition)
    {
        float offset = 0;
        if ((int)worldPosition.x % 2 == 0 && (int)worldPosition.z % 3 != 0)
            offset = TileMap.hexWidth / 2;

        float x = (float)((Math.Round(worldPosition.x) - TileMap.startPos.x - offset) / TileMap.hexWidth);
        float z = (worldPosition.z - TileMap.startPos.z) / (TileMap.hexHeight * 0.75f) * (-1.0f);

        return new LocalPosition((int)Math.Round(x), (int)Math.Round(z));
    }
}

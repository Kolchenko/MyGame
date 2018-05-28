using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

[System.Serializable]
public struct LocalPosition
{
    public int x;
    public int y;

    public LocalPosition(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
}

[System.Serializable]
public struct WorldPosition
{
    public float x;
    public float y;
    public float z;

    public WorldPosition(float x, int y, float z) : this()
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    public Vector3 ToVector3()
    {
        return new Vector3(x, y, z);
    }
}

public class Unit : MonoBehaviour {
    public LocalPosition localPosition;
    public WorldPosition worldPosition;
    public bool isBotUnit;
    public int distance;
    public int damage;
    public int defense;

    public List<Node> currentPath;

    public Color startColor;
    public Color availableTileColor;
    public List<Node> availableMovementTiles = null;

    public void UpdatePosition(WorldPosition worldPosition)
    {
        this.worldPosition = worldPosition;
        this.localPosition = PositionConverter.ToLocalCoordinates(worldPosition);
    }

    public void MoveToEnterTile()
    {
        if (currentPath != null)
        {
            WorldPosition moveTo = PositionConverter.ToWorldCoordinates(new LocalPosition(currentPath[currentPath.Count - 1].x, currentPath[currentPath.Count - 1].y));
            moveTo.y = transform.position.y;
            UpdatePosition(moveTo);
            transform.position = moveTo.ToVector3();
            //StartCoroutine(MoveObject.MoveUnit(moveTo)); //TODO: return
        }
    }

    public static bool isHumanMakeTurn = false;
}

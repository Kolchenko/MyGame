using UnityEngine;
using System.Collections;

[System.Serializable]
public class TileType
{
    public string name;
    public GameObject tileVisualPrefab;
    public int movementCost;
    public Color originalColor;

    public static int GetTileTypeByTileName(string name)
    {
        if (name == "grass")
        {
            return 0;
        } 
        else if (name == "mountain")
        {
            return 1;
        }

        return 0;
    }
}

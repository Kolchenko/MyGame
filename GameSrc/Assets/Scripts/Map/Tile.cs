using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour {
    public void Create(Transform hex, Vector2 position)
    {
        Transform tile = Instantiate(hex) as Transform;

        tile.position = TileMap.Instance.CalcWorldPos(position);
        tile.parent = TileMap.Instance.transform;
        tile.name = "Hexagon" + position.x + "|" + position.y;

        ClickableTile ct;
        if (tile.GetComponent<ClickableTile>() != null)
        {
            ct = tile.GetComponent<ClickableTile>();
            ct.tileX = tile.position.x;
            ct.tileY = tile.position.z;
        }
    }

    public void Select(Color startColor, Color mouseOverColor)
    {        
        GetComponent<Renderer>().material.SetColor("_Color", mouseOverColor);
    }

    public void Deselect(Color startColor)
    {
        GetComponent<Renderer>().material.SetColor("_Color", startColor);
    }
}

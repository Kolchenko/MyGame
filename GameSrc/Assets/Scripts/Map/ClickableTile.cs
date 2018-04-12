using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickableTile : MonoBehaviour {
    public float tileX;
    public float tileY;
    public TileMap map;

    private void OnMouseUp()
    {
        Debug.Log("Tile click");
        map.MoveSelectedUnitTo(tileX, tileY);
    }
}

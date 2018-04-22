using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickableTile : MonoBehaviour {
    public float tileX;
    public float tileY;

    private void OnMouseUp()
    {
        if (TileMap.selectedUnit != null)
        {
            TileMap.Instance.GeneratePathTo(tileX, tileY);
            TileMap.selectedUnit.MoveToEnterTile();
        }
    }
}

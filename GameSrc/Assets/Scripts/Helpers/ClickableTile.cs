using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickableTile : MonoBehaviour {
    public float tileX;
    public float tileY;

    private void OnMouseUp()
    {
        if (BoardManager.selectedUnit != null)
        {
            BoardManager.Instance.GeneratePathTo(tileX, tileY);
            BoardManager.selectedUnit.MoveToEnterTile();
        }
    }
}

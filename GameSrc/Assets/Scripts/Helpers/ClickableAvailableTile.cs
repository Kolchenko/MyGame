using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickableAvailableTile : MonoBehaviour {
    public float tileX;
    public float tileY;

    private void OnMouseUp()
    {
        Vector2 clickedTileLocalPos = PositionConverter.ToLocalCoordinates(new Vector3(tileX, 0, tileY));
        Node clickedTtile = BoardManager.Instance.map.graph[(int)clickedTileLocalPos.x, (int)clickedTileLocalPos.y];

        if (BoardManager.selectedUnit != null && BoardManager.Instance.isAvailableClickedTile(clickedTtile))
        {
            BoardManager.Instance.GeneratePathTo(tileX, tileY);
            BoardManager.selectedUnit.MoveToEnterTile();
            Unit.isHumanMakeTurn = true;
        }
    }
}

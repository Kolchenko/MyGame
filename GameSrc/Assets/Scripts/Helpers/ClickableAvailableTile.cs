using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickableAvailableTile : MonoBehaviour {
    public float tileX;
    public float tileY;

    private void OnMouseUp()
    {
        LocalPosition clickedTileLocalPos = PositionConverter.ToLocalCoordinates(new WorldPosition(tileX, 0, tileY));
        Node clickedTtile = BoardManager.Instance.map.graph[clickedTileLocalPos.x, clickedTileLocalPos.y];

        if (BoardManager.selectedUnit != null && BoardManager.Instance.isAvailableClickedTile(clickedTtile))
        {
            BoardManager.Instance.GeneratePathTo(tileX, tileY);
            BoardManager.selectedUnit.MoveToEnterTile();
            Unit.isHumanMakeTurn = true;
            MouseMoveableTile.isOnMouseUp = true;
        }
    }
}

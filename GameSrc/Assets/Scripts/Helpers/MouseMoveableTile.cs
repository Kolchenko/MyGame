using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseMoveableTile : MonoBehaviour {
    public Color startColor;
    public Color mouseOverColor;
    public static bool isOnMouseUp = false;
    private static Renderer currentTile;

    private void OnMouseEnter()
    {
        //if (!PauseMenu.gameIsPaused)
        //{
        //    currentTile = GetComponent<Renderer>();
        //    startColor = currentTile.material.GetColor("_Color");
        //    GameObjectHighlighter.Select(startColor, mouseOverColor, GetComponent<Renderer>());
        //}
    }

    private void OnMouseExit()
    {
        // after click on the tile
        //if (!isOnMouseUp && !PauseMenu.gameIsPaused)
        //{
        //    GameObjectHighlighter.Deselect(startColor, GetComponent<Renderer>());
        //}
        //isOnMouseUp = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && currentTile != null)
        {
            GameObjectHighlighter.Deselect(startColor, currentTile);            
        }
    }
}

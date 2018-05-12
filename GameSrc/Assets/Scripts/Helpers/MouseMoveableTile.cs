using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseMoveableTile : MonoBehaviour {
    Color startColor;
    public Color mouseOverColor;
    public static bool isOnMouseUp = false;

    private void OnMouseEnter()
    {
        startColor = GetComponent<Renderer>().material.GetColor("_Color");
        GameObjectHighlighter.Select(startColor, mouseOverColor, GetComponent<Renderer>());
    }

    private void OnMouseExit()
    {
        // after click on the tile
        if (!isOnMouseUp)
        {
            GameObjectHighlighter.Deselect(startColor, GetComponent<Renderer>());
        }
        isOnMouseUp = false;
    }
}

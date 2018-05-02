using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseMoveableTile : MonoBehaviour {
    Color startColor;
    public Color mouseOverColor;

    private void OnMouseEnter()
    {
        startColor = GetComponent<Renderer>().material.GetColor("_Color");
        GameObjectHighlighter.Select(startColor, mouseOverColor, GetComponent<Renderer>());
    }

    private void OnMouseExit()
    {
        GameObjectHighlighter.Deselect(startColor, GetComponent<Renderer>());
    }
}

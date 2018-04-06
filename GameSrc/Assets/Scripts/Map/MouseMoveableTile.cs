using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseMoveableTile : MonoBehaviour {
    Color startColor;
    public Color mouseOverColor;

    private void OnMouseEnter()
    {
        startColor = GetComponent<Renderer>().material.GetColor("_Color");
        GetComponent<Renderer>().material.SetColor("_Color", mouseOverColor);
    }

    private void OnMouseExit()
    {
        GetComponent<Renderer>().material.SetColor("_Color", startColor);
    }
}

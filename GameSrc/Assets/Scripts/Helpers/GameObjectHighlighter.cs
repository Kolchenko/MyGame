using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

class GameObjectHighlighter
{
    public static void Select(Color startColor, Color mouseOverColor, Renderer renderer)
    {
        renderer.material.SetColor("_Color", mouseOverColor);
    }

    public static void Deselect(Color startColor, Renderer renderer)
    {
        renderer.material.SetColor("_Color", startColor);
    }
}

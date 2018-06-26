using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingTextController : MonoBehaviour {
    private static FloatingText popupText;
    private static GameObject canvas;
    public static bool isInitialize = false;
    
    public static void Initialize()
    {
        isInitialize = true;
        Time.timeScale = 1;
        canvas = GameObject.Find("Canvas");
        if (!popupText)
            popupText = Resources.Load<FloatingText>("Prefabs/PopupTextParent");
    }

    public static void CreateFloatingText(string text, WorldPosition position)
    {
        FloatingText instance = Instantiate(popupText);
        instance.transform.SetParent(canvas.transform, false);
        instance.transform.position = Camera.main.WorldToScreenPoint(position.ToVector3());
        instance.SetDamageText(text);
    }
}

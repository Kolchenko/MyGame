using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    public static GameManager Instance { get; private set; }
   
    public void Awake()
    {
        Debug.Log("GameManagerAwake");
        Instance = this;
    }
}

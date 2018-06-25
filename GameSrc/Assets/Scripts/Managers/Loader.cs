using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loader : MonoBehaviour {
    public GameObject gameManager;


    void Awake () {
        Debug.Log("LoaderAwake");
		if (GameManager.Instance == null)
        {
            Debug.Log("Instantiate");
            Instantiate(gameManager);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour {
    public TileMap map;
    public int tileX;
    public int tileY;
    public int maxDistance = 3;

    private void OnMouseUp()
    {
        if (Input.GetMouseButtonUp(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100))
            {
                Debug.Log(hit.transform.gameObject.name);
                map.setSelectedUnit(hit.transform.gameObject.GetComponent<Unit>());
            }
        }
    }
}

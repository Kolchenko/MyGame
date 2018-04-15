using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour {
    public TileMap map;
    public float tileX;
    public float tileZ;
    public int maxDistance = 3;

    private void OnMouseUp()
    {
        map.resetSelectedUnit();
        if (Input.GetMouseButtonUp(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100))
            {
                Debug.Log(hit.transform.gameObject.name);
                map.setSelectedUnit(hit.transform.gameObject.GetComponent<Unit>());
            }


            Vector2 tilePos = map.GetTileCoordinatesByWorldPosition(new Vector3(tileX, 0, tileZ));
        }
    }

    public void UpdatePosition(float x, float z)
    {
        tileX = x;
        tileZ = z;
    }
}

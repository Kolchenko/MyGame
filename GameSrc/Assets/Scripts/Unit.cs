using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Unit : MonoBehaviour {
    public float tileX;
    public float tileZ;
    public int maxDistance;
    public List<Node> currentPath;

    private void OnMouseUp()
    {
        TileMap map = TileMap.Instance;
        map.DeselectUnit();
        if (Input.GetMouseButtonUp(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100))
            {
                Debug.Log(hit.transform.gameObject.name);
                map.SelectUnit(hit.transform.gameObject.GetComponent<Unit>());
            }
            
            //Vector2 unitPosition = map.GetTileCoordinatesByWorldPosition(new Vector3(tileX, 0, tileZ));

            //List<TileMap.Node> unitTileNeighbours = map.getNeighboursByUnitPos(unitPosition);
            //foreach (var item in unitTileNeighbours)
            //{
                
            //} 
        }
    }

    private void Update()
    {
        if (currentPath != null)
        {
            int currentNode = 0;

            while(currentNode < currentPath.Count - 1)
            {
                Vector3 start = TileMap.Instance.CalcWorldPos(new Vector2(currentPath[currentNode].x, currentPath[currentNode].y)) + new Vector3(0, 0.2f, 0);
                Vector3 end = TileMap.Instance.CalcWorldPos(new Vector2(currentPath[currentNode + 1].x, currentPath[currentNode + 1].y)) + new Vector3(0, 0.2f, 0);
                Debug.DrawLine(start, end, Color.blue);
                ++currentNode;
            }
        }
    }

    public void UpdatePosition(float x, float z)
    {
        tileX = x;
        tileZ = z;
    }

    public void MoveToEnterTile()
    {
        if (currentPath == null)
        {
            return;
        }

        while (currentPath.Count != 1)
        {
            currentPath.RemoveAt(0);
            transform.position = TileMap.Instance.CalcWorldPos(new Vector2(currentPath[0].x, currentPath[0].y));
        }

        currentPath = null;
    }
}

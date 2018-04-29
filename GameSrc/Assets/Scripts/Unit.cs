using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Unit : MonoBehaviour {
    public float tileX;
    public float tileZ;
    public int maxDistance;
    public List<Node> currentPath;

    Color startColor;
    public Color mouseOverColor;

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

            Vector2 unitPosition = map.GetTileCoordinatesByWorldPosition(new Vector3(tileX, 0, tileZ));
            //int[,] availableTiles = map.getAvailableTilesByUnitPos(unitPosition);
            List<Node> availableMovementTiles = new List<Node>();
            Node currentTile = map.graph[(int)unitPosition.x, (int)unitPosition.y];
            GetAvailableMovementTiles(availableMovementTiles, currentTile);

            foreach (var item in availableMovementTiles)
            {
                Collider[] colliders;
                Vector3 tileWorldPos = map.CalcWorldPos(new Vector2(item.x, item.y));
                colliders = Physics.OverlapSphere(tileWorldPos, 0.125f /* Radius */);

                if (colliders.Length >= 1) 
                {
                    foreach (var collider in colliders)
                    {
                        var go = collider.gameObject; //This is the game object you collided with
                        if (go.name.StartsWith("Hexagon"))
                        {
                            startColor = go.GetComponent<Renderer>().material.GetColor("_Color");
                            go.GetComponent<Tile>().Select(startColor, mouseOverColor);
                        }
                    }
                }
            }
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

    private void GetAvailableMovementTiles(List<Node> availableMovementTiles, Node startPos)
    {
        TileMap map = TileMap.Instance;
        int maxDepth = maxDistance;
        int currentDepth = 0;
        int elementsToDepthIncrease = 1;
        int nextElementsToDepthIncrease = 0;

        Queue<Node> burningTiles = new Queue<Node>();
        burningTiles.Enqueue(startPos);   
        
        List<Node> visitedTiles = new List<Node>();
        visitedTiles.Add(startPos);

        while (burningTiles.Count != 0)
        {            
            Node currentVertex = burningTiles.Dequeue();
            foreach (var item in currentVertex.neighbours)
            {
                if (!visitedTiles.Contains(item))
                {
                    ++nextElementsToDepthIncrease;
                }
            }

            if (--elementsToDepthIncrease == 0)
            {
                elementsToDepthIncrease = nextElementsToDepthIncrease;
                nextElementsToDepthIncrease = 0;
                ++currentDepth;
            }

            for (int i = 0; i < currentVertex.neighbours.Count; ++i)
            {
                Node tile = currentVertex.neighbours[i];
                if (!visitedTiles.Contains(tile))
                {
                    visitedTiles.Add(tile);
                    burningTiles.Enqueue(tile);
                    availableMovementTiles.Add(tile);
                }
            }

            if (currentDepth == maxDepth)
            {
                return;
            }
        }
    }
}

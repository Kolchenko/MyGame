using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public abstract class Unit : MonoBehaviour {
    public float tileX;
    public float tileZ;

    public int distance;
    public int damage;
    public int defense;

    public List<Node> currentPath;

    public Color startColor;
    public Color availableTileColor;
    public List<Node> availableMovementTiles = null;

    private void OnMouseUp()
    {
        BoardManager board = BoardManager.Instance;
        TileMap map = board.map;

        board.DeselectUnit();
        if (Input.GetMouseButtonUp(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100))
            {
                Debug.Log(hit.transform.gameObject.name);
                board.SelectUnit(hit.transform.gameObject.GetComponent<Unit>());
            }

            Vector2 unitPosition = PositionConverter.ToLocalCoordinates(new Vector3(tileX, 0, tileZ));
            availableMovementTiles = new List<Node>();
            Node currentTile = map.graph[(int)unitPosition.x, (int)unitPosition.y];
            board.GetAvailableMovementTiles(availableMovementTiles, currentTile);

            foreach (var item in availableMovementTiles)
            {
                Collider[] colliders;
                Vector3 tileWorldPos = PositionConverter.ToWorldCoordinates(new Vector2(item.x, item.y));
                colliders = Physics.OverlapSphere(tileWorldPos, 0.125f /*Radius*/);

                if (colliders.Length >= 1) 
                {
                    foreach (var collider in colliders)
                    {
                        var go = collider.gameObject;
                        if (go.name.StartsWith("Hexagon"))
                        {
                            if (map.tiles[item.x, item.y] != 1 /*mountain*/)
                            {
                                startColor = go.GetComponent<Renderer>().material.GetColor("_Color");
                                availableTileColor = startColor;
                                //TODO: think about  other variant of colorized tile
                                availableTileColor.g = 0.6f;
                                availableTileColor.b = 0.4f;
                                go.GetComponent<Tile>().Select(startColor, availableTileColor);
                            }
                        }
                    }
                }
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
        float y = transform.position.y;
        while (currentPath.Count != 1)
        {
            currentPath.RemoveAt(0);
            Vector3 unitPos = PositionConverter.ToWorldCoordinates(new Vector2(currentPath[0].x, currentPath[0].y));
            unitPos.y = y;
            transform.position = unitPos;
        }

        currentPath = null;
        UpdatePosition(transform.position.x, transform.position.z);
        BoardManager.Instance.DeselectUnit();
        availableMovementTiles = null;
    }
}

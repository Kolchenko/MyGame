﻿using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Unit : MonoBehaviour {
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
                                GameObjectHighlighter.Select(startColor, availableTileColor, go.GetComponent<Renderer>());
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
        Vector3 moveTo = PositionConverter.ToWorldCoordinates(new Vector2(currentPath[currentPath.Count - 1].x, currentPath[currentPath.Count - 1].y));
        moveTo.y = transform.position.y;
        StartCoroutine(MoveObject.MoveUnit(moveTo));
    }
}

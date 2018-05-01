using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour {
    public Unit[] playerUnits;
    public Unit[] aiUnits;

    static public Unit selectedUnit;
    static Color startPlayerColor;
    static Color selectedPlayerColor = new Color(0.1f, 0.1f, 0.9f);
    List<Node> currentPath = null;
    public TileMap map = null;

    public static BoardManager Instance { get; private set; }

    public void Awake()
    {
        Debug.Log("BoardManagerAwake");
        Instance = this;
        map = GetComponent<TileMap>();
    }

    public void SelectUnit(Unit unit)
    {
        if (unit != null)
        {
            selectedUnit = unit;
            startPlayerColor = selectedUnit.GetComponent<Renderer>().material.GetColor("_Color");
            selectedUnit.GetComponent<Renderer>().material.SetColor("_Color", selectedPlayerColor);
        }
    }

    public void DeselectUnit()
    {
        if (selectedUnit != null)
        {
            selectedUnit.GetComponent<Renderer>().material.SetColor("_Color", startPlayerColor);
            DeselecteAvailableTile();
            selectedUnit = null;
        }
    }

    public void GeneratePathTo(float x, float z)
    {
        selectedUnit.currentPath = null;
        Dictionary<Node, float> dist = new Dictionary<Node, float>();
        Dictionary<Node, Node> prev = new Dictionary<Node, Node>();
        List<Node> unvisited = new List<Node>();

        Vector2 sourceLocalPos = PositionConverter.ToLocalCoordinates(new Vector3(selectedUnit.tileX, 0, selectedUnit.tileZ));
        Vector2 targetLocalPos = PositionConverter.ToLocalCoordinates(new Vector3(x, 0, z));

        Node source = map.graph[(int)sourceLocalPos.x, (int)sourceLocalPos.y];
        Node target = map.graph[(int)targetLocalPos.x, (int)targetLocalPos.y];

        if (selectedUnit.availableMovementTiles.Contains(target))
        {

            dist[source] = 0;
            prev[source] = null;

            foreach (var vertex in map.graph)
            {
                if (vertex != source)
                {
                    dist[vertex] = Mathf.Infinity;
                    prev[vertex] = null;
                }
                unvisited.Add(vertex);
            }

            while (unvisited.Count > 0)
            {
                Node u = null;

                foreach (var possibleU in unvisited)
                {
                    if (u == null || dist[u] > dist[possibleU])
                    {
                        u = possibleU;
                    }
                }

                if (u == target)
                {
                    break;
                }

                unvisited.Remove(u);

                foreach (var v in u.neighbours)
                {
                    //float alt = dist[u] + u.DistanceTo(v); //without calc tile cost
                    float alt = dist[u] + map.CostToEnterTile(v.x, v.y);
                    if (alt < dist[v])
                    {
                        dist[v] = alt;
                        prev[v] = u;
                    }
                }
            }

            if (prev[target] == null)
            {
                //No route between source and target
                return;
            }
            else
            {
                currentPath = new List<Node>();
                Node currentTile = target;

                while (currentTile != null)
                {
                    currentPath.Add(currentTile);
                    currentTile = prev[currentTile];
                }

                currentPath.Reverse();
                selectedUnit.currentPath = currentPath;
            }
        }
    }

    public void GetAvailableMovementTiles(List<Node> availableMovementTiles, Node startPos)
    {
        int maxDepth = selectedUnit.distance;
        int currentDepth = 0;
        int elementsToDepthIncrease = 1;
        int nextElementsToDepthIncrease = 0;

        Queue<Node> burningTiles = new Queue<Node>();
        burningTiles.Enqueue(startPos);

        List<Node> visitedTiles = new List<Node>();
        visitedTiles.Add(startPos);
        availableMovementTiles.Add(startPos);

        // check other player units pos
        foreach (var item in BoardManager.Instance.playerUnits)
        {
            Vector2 unitPosition = PositionConverter.ToLocalCoordinates(new Vector3(item.tileX, 0, item.tileZ));
            Node currentTileWithUnit = map.graph[(int)unitPosition.x, (int)unitPosition.y];
            visitedTiles.Add(currentTileWithUnit);
        }

        while (burningTiles.Count != 0)
        {
            Node currentVertex = burningTiles.Dequeue();
            foreach (var item in currentVertex.neighbours)
            {
                if (!visitedTiles.Contains(item) && map.tiles[item.x, item.y] != 1)
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
                if (!visitedTiles.Contains(tile) && map.tiles[tile.x, tile.y] != 1 /*mountain*/)
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

    public void DeselecteAvailableTile()
    {
        foreach (var item in selectedUnit.availableMovementTiles)
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
                            go.GetComponent<Tile>().Deselect(selectedUnit.startColor);
                        }
                    }
                }
            }
        }
    }

}

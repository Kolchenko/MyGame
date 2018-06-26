using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BoardManager : MonoBehaviour {
    public List<Unit> playerUnits;
    public List<Unit> enemyUnits;

    static public Unit selectedUnit;
    public static Color startPlayerColor;
    public static Color selectedPlayerColor = new Color(0.1f, 0.1f, 0.9f);
    public Transform target = null;
    List<Node> currentPath = null;
    List<Node> currentPathToEnemy = null;
    public TileMap map = null;

    public TextMeshProUGUI BlueSpearmansValue;
    public TextMeshProUGUI BlueBowmansValue;
    public TextMeshProUGUI BlueGriffinsValue;
    public TextMeshProUGUI RedSpearmansValue;
    public TextMeshProUGUI RedBowmansValue;
    public TextMeshProUGUI RedGriffinsValue;

    public static BoardManager Instance { get; private set; }

    public void Awake()
    {
        Debug.Log("BoardManagerAwake");
        Instance = this;
        map = GetComponent<TileMap>();
    }

    private void Start()
    {
        //FloatingTextController.Initialize();
    }

    public void SelectUnit(Unit unit)
    {
        if (unit != null)
        {
            selectedUnit = unit;
            SelectAvailableTile();
            startPlayerColor = selectedUnit.GetComponent<Renderer>().material.GetColor("_Color");
            selectedUnit.GetComponent<Renderer>().material.SetColor("_Color", selectedPlayerColor);
        }
    }

    public void DeselectUnit()
    {
        if (selectedUnit != null)
        {
            DeselecteAvailableTile();
            selectedUnit.GetComponent<Renderer>().material.SetColor("_Color", startPlayerColor);
            selectedUnit = null;
        }
    }

    public void SelectAvailableTile()
    {
        LocalPosition unitPosition = PositionConverter.ToLocalCoordinates(selectedUnit.worldPosition);
        selectedUnit.availableMovementTiles = new List<Node>();
        Node currentTile = map.graph[unitPosition.x, unitPosition.y];
        GetAvailableMovementTiles(selectedUnit.availableMovementTiles, currentTile);

        foreach (var item in selectedUnit.availableMovementTiles)
        {
            Collider[] colliders;
            WorldPosition tileWorldPos = PositionConverter.ToWorldCoordinates(new LocalPosition(item.x, item.y));
            colliders = Physics.OverlapSphere(tileWorldPos.ToVector3(), 0.125f /*Radius*/);

            if (colliders.Length >= 1)
            {
                foreach (var collider in colliders)
                {
                    var go = collider.gameObject;
                    if (go.name.StartsWith("Hexagon"))
                    {
                        if (map.tiles[item.x, item.y] != (int)TileTypes.SWAMP)
                        {
                            selectedUnit.startColor = go.GetComponent<Renderer>().material.GetColor("_Color");
                            selectedUnit.availableTileColor = selectedUnit.startColor;
                            //TODO: think about  other variant of colorized tile
                            selectedUnit.availableTileColor.g = 0.6f;
                            selectedUnit.availableTileColor.b = 0.4f;
                            GameObjectHighlighter.Select(selectedUnit.startColor, selectedUnit.availableTileColor, go.GetComponent<Renderer>());
                        }
                    }
                }
            }
        }
    }

    public void DeselecteAvailableTile()
    {
        if (selectedUnit.availableMovementTiles != null)
        {
            foreach (var item in selectedUnit.availableMovementTiles)
            {
                Collider[] colliders;
                WorldPosition tileWorldPos = PositionConverter.ToWorldCoordinates(new LocalPosition(item.x, item.y));
                colliders = Physics.OverlapSphere(tileWorldPos.ToVector3(), 0.125f /*Radius*/);

                if (colliders.Length >= 1)
                {
                    foreach (var collider in colliders)
                    {
                        var gameObject = collider.gameObject;
                        if (map.tiles[item.x, item.y] != (int)TileTypes.SWAMP)
                        {
                            GameObjectHighlighter.Deselect(selectedUnit.startColor, gameObject.GetComponent<Renderer>());
                        }
                    }
                }
            }
        }
    }

    public void GeneratePathTo(float x, float z)
    {
        selectedUnit.currentPath = null;
        Dictionary<Node, float> dist = new Dictionary<Node, float>();
        Dictionary<Node, Node> prev = new Dictionary<Node, Node>();
        List<Node> unvisited = new List<Node>();

        LocalPosition sourceLocalPos = PositionConverter.ToLocalCoordinates(selectedUnit.worldPosition);
        LocalPosition targetLocalPos = PositionConverter.ToLocalCoordinates(new WorldPosition(x, 0, z));

        Node source = map.graph[sourceLocalPos.x, sourceLocalPos.y];
        Node target = map.graph[targetLocalPos.x, targetLocalPos.y];

        if (selectedUnit.availableMovementTiles == null)
        {
            selectedUnit.availableMovementTiles = new List<Node>();
            Node currentTile = map.graph[selectedUnit.localPosition.x, selectedUnit.localPosition.y];
            GetAvailableMovementTiles(selectedUnit.availableMovementTiles, currentTile);
        }

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
                    if (selectedUnit.availableMovementTiles.Contains(v))
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

    public void GeneratePathToEnemy(Unit enemy)
    {
        selectedUnit.currentPathToEnemy = null;
        // check the neighborhood of units
        if (selectedUnit.DistanceTo(enemy) == 1)
        {
            Node enemyTile = map.graph[enemy.localPosition.x, enemy.localPosition.y];
            List<Node> enemyTileNeighbours = enemyTile.neighbours;
            Node selectedUnitTile = map.graph[selectedUnit.localPosition.x, selectedUnit.localPosition.y];
            foreach (var item in enemyTileNeighbours)
            {
                if (selectedUnitTile.DistanceBetweenNode(item) == 1 && 
                    map.tiles[item.x, item.y] != (int)TileTypes.SWAMP &&
                    !isUnitOccupiedNode(item))
                {
                    currentPathToEnemy = new List<Node>();
                    currentPathToEnemy.Add(item);
                    selectedUnit.currentPathToEnemy = currentPathToEnemy;
                    return;
                }
            }
        }
        Dictionary<Node, float> dist = new Dictionary<Node, float>();
        Dictionary<Node, Node> prev = new Dictionary<Node, Node>();
        List<Node> unvisited = new List<Node>();

        LocalPosition sourceLocalPos = PositionConverter.ToLocalCoordinates(selectedUnit.worldPosition);
        LocalPosition targetLocalPos = PositionConverter.ToLocalCoordinates(enemy.worldPosition);

        Node source = map.graph[sourceLocalPos.x, sourceLocalPos.y];
        Node target = map.graph[targetLocalPos.x, targetLocalPos.y];
        Node currentUnitPos = map.graph[selectedUnit.localPosition.x, selectedUnit.localPosition.y];
        if (selectedUnit.availableMovementTiles == null)
        {
            selectedUnit.availableMovementTiles = new List<Node>();
        }

        GetAvailableMovementTiles(selectedUnit.availableMovementTiles, currentUnitPos);

        if (selectedUnit.CanAttack(enemy))
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
                    if (selectedUnit.availableMovementTiles.Contains(v) || (v.x == enemy.localPosition.x && v.y == enemy.localPosition.y))
                    {
                        float alt = dist[u] + u.DistanceTo(v); //without calc tile cost                        
                        if (alt < dist[v])
                        {
                            dist[v] = alt;
                            prev[v] = u;
                        }
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
                currentPathToEnemy = new List<Node>();
                Node currentTile = target;

                while (currentTile != null)
                {
                    currentPathToEnemy.Add(currentTile);
                    currentTile = prev[currentTile];
                }

                currentPathToEnemy.Reverse();
                currentPathToEnemy.RemoveAt(currentPathToEnemy.Count - 1);
                selectedUnit.currentPathToEnemy = currentPathToEnemy;
            }
        }
    }

    //todo: think about signature
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

        // check other player units pos
        foreach (var item in Instance.playerUnits)
        {
            LocalPosition unitPosition = PositionConverter.ToLocalCoordinates(item.worldPosition);
            Node currentTileWithUnit = map.graph[unitPosition.x, unitPosition.y];
            visitedTiles.Add(currentTileWithUnit);
        }

        // check other AI units pos
        foreach (var item in Instance.enemyUnits)
        {
            LocalPosition unitPosition = PositionConverter.ToLocalCoordinates(item.worldPosition);
            Node currentTileWithUnit = map.graph[unitPosition.x, unitPosition.y];
            visitedTiles.Add(currentTileWithUnit);
        }

        while (burningTiles.Count != 0)
        {
            Node currentVertex = burningTiles.Dequeue();
            foreach (var item in currentVertex.neighbours)
            {
                if (!visitedTiles.Contains(item) && map.tiles[item.x, item.y] != (int)TileTypes.SWAMP)
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
                if ((!visitedTiles.Contains(tile) && map.tiles[tile.x, tile.y] != 1 /*mountain*/))
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

    public List<Unit> GetAvailableHumanUnitsForAttack()
    {
        List<Unit> availableUnit = new List<Unit>();
        foreach (var item in Instance.playerUnits)
        {
            int distance = selectedUnit.DistanceTo(item);
            if (selectedUnit.distance >= distance || selectedUnit.tag == "Bowman")
            {
                availableUnit.Add(item);
            }
        }

        return availableUnit;
    }

    public bool isAvailableClickedTile(Node tile)
    {
        return selectedUnit.availableMovementTiles.Contains(tile);
    }

    public bool isUnitOccupiedNode(Node node)
    {
        foreach (var item in playerUnits)
        {
            if (item.localPosition.x == node.x && item.localPosition.y == node.y)
            {
                return true;
            }
        }

        foreach (var item in enemyUnits)
        {
            if (item.localPosition.x == node.x && item.localPosition.y == node.y)
            {
                return true;
            }
        }

        return false;
    }

    public bool isUnitOccupiedNodeByTeam(Node node, bool isBotTeam)
    {
        List<Unit> team = !isBotTeam ? enemyUnits : playerUnits;        
        foreach (var item in playerUnits)
        {
            if (item.localPosition.x == node.x && item.localPosition.y == node.y)
            {
                return true;
            }
        }

        return false;
    } 

    public Unit GetUnitByNode(Node node, bool isBotTeam)
    {
        List<Unit> team = isBotTeam ? enemyUnits : playerUnits;
        foreach (var item in team)
        {
            if (item.localPosition.x == node.x && item.localPosition.y == node.y)
            {
                return item;
            }
        }

        return null;
    }

    public void RewriteCountOfWarriorValue(Unit unit, int countOfWarrior)
    {
        countOfWarrior = countOfWarrior < 0 ? 0 : countOfWarrior;
        if (unit.isBotUnit)
        {
            switch (unit.tag)
            {
                case "Spearman":
                    RedSpearmansValue.text = countOfWarrior.ToString();
                    break;
                case "Bowman":
                    RedBowmansValue.text = countOfWarrior.ToString();
                    break;
                case "Griffin":
                    RedGriffinsValue.text = countOfWarrior.ToString();
                    break;
                default:
                    break;
            }
        }
        else
        {
            switch (unit.tag)
            {
                case "Spearman":
                    BlueSpearmansValue.text = countOfWarrior.ToString();
                    break;
                case "Bowman":
                    BlueBowmansValue.text = countOfWarrior.ToString();
                    break;
                case "Griffin":
                    BlueGriffinsValue.text = countOfWarrior.ToString();
                    break;
                default:
                    break;
            }
        }
    }
}

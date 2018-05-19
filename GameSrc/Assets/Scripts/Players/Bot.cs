using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Bot {
    public class Pair<T, U>
    {
        public Pair()
        {
        }

        public Pair(T first, U second)
        {
            this.First = first;
            this.Second = second;
        }

        public T First { get; set; }
        public U Second { get; set; }
    };

    private List<Unit> botTeam;

    public Bot()
    {
        botTeam = BoardManager.Instance.enemyUnits.OfType<Unit>().ToList();
    }

    public void Do()
    {
        MakeBestMove();
        Turn.isHumanTurn = true;
    }

    private void MakeBestMove()
    {
        Dictionary<Unit, List<Node>> availableMovementTilesByUnit = GetAllAvailableTeamMoves(botTeam);
        
    }

    private void ScoreCurrentUnitPos(List<Node> availableMoves)
    {

    }

    private float GetHeuristicEvaluation(Node node, bool isBotUnit)
    {
        //TODO: temporary evalute        
        int x = BoardManager.Instance.map.width - node.x;
        int value = isBotUnit ? x : x * -1;
        return value;
    }

    private Dictionary<Unit, List<Node>> GetAllAvailableTeamMoves(List<Unit> team)
    {
        Dictionary<Unit, List<Node>> availableMovementTilesByUnit = new Dictionary<Unit, List<Node>>();
        List<List<Node>> result = new List<List<Node>>();
        foreach (var item in team)
        {
            BoardManager.selectedUnit = item;
            List<Node> moves = new List<Node>();
            Vector2 localPos = PositionConverter.ToLocalCoordinates(new Vector3(item.tileX, 0, item.tileZ));
            BoardManager.Instance.GetAvailableMovementTiles(moves, BoardManager.Instance.map.graph[(int)localPos.x, (int)localPos.y]);
            result.Add(moves);
        }

        var x = AllCombinationsOf(result);

        return availableMovementTilesByUnit;
    }

    public static List<List<T>> AllCombinationsOf<T>(List<List<T>> sets)
    {
        var combinations = new List<List<T>>();

        foreach (var value in sets[0])
            combinations.Add(new List<T> { value });

        foreach (var set in sets.Skip(1))
            combinations = AddExtraSet(combinations, set);

        return combinations;
    }

    private static List<List<T>> AddExtraSet<T>
         (List<List<T>> combinations, List<T> set)
    {
        var newCombinations = from value in set
                              from combination in combinations
                              select new List<T>(combination) { value };

        return newCombinations.ToList();
    }
}

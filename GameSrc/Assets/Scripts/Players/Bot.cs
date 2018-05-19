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

    bool isBotTeam = true;

    public void Do()
    {
        List<Node> startTeamPos = new List<Node>();
        foreach (var unit in botTeam)
        {
            Vector2 unitLocalPos = PositionConverter.ToLocalCoordinates(new Vector3(unit.tileX, 0, unit.tileZ));
            Node unitStartNode = BoardManager.Instance.map.graph[(int)unitLocalPos.x, (int)unitLocalPos.y];
            startTeamPos.Add(unitStartNode);
        }

        var bestMove = MakeBestMove(startTeamPos, 2, isBotTeam);

        for (int i = 0; i < botTeam.Count; ++i)
        {
            float y = botTeam[i].transform.position.y;
            Vector3 unitWorldPos = PositionConverter.ToWorldCoordinates(new Vector2(bestMove.Second[i].x, bestMove.Second[i].y));
            unitWorldPos.y = y;
            botTeam[i].transform.position = unitWorldPos;
            botTeam[i].UpdatePosition(botTeam[i].transform.position.x, botTeam[i].transform.position.z);
        }

        Turn.isHumanTurn = true;
    }

    float maxScore = Mathf.NegativeInfinity;
    List<Node> bestTeamPos = new List<Node>();

    private Pair<float, List<Node>> MakeBestMove(List<Node> teamPos, int depth, bool isBotTeam)
    {
        if (depth == 0)
        {
            return GetHeuristicEvaluation(teamPos, isBotTeam);
        }

        isBotTeam = !isBotTeam;

        List<List<Node>> allAvailableTeamMoves = new List<List<Node>>();
        if (isBotTeam)
        {
            allAvailableTeamMoves = GetAllAvailableTeamMoves(botTeam);
        }
        else
        {
            allAvailableTeamMoves = GetAllAvailableTeamMoves(Human.humanTeam);
        }

        foreach (var teamPosition in allAvailableTeamMoves)
        {
            Pair<float, List<Node>> tmpResult = MakeBestMove(teamPosition, depth - 1, isBotTeam);
            if (tmpResult.First > maxScore)
            {
                maxScore = tmpResult.First;
                bestTeamPos = tmpResult.Second;
            }
        }

        return new Pair<float, List<Node>>(maxScore, bestTeamPos);
    }

    private Pair<float, List<Node>> GetHeuristicEvaluation(List<Node> teamPosition, bool isBotTeam)
    {
        int value = 0;
        foreach (var unitPosition in teamPosition)
        {
            value += BoardManager.Instance.map.width - unitPosition.x;
        }
        
        value = isBotTeam ? value : value * -1;
        return new Pair<float, List<Node>> (value, teamPosition);
    }


    #region get all moves

    private List<List<Node>> GetAllAvailableTeamMoves(List<Unit> team)
    {
        List<List<Node>> allTeamMoves = new List<List<Node>>();
        foreach (var item in team)
        {
            BoardManager.selectedUnit = item;
            List<Node> moves = new List<Node>();
            Vector2 localPos = PositionConverter.ToLocalCoordinates(new Vector3(item.tileX, 0, item.tileZ));
            BoardManager.Instance.GetAvailableMovementTiles(moves, BoardManager.Instance.map.graph[(int)localPos.x, (int)localPos.y]);
            allTeamMoves.Add(moves);
        }

        List<List<Node>> allCombinationMoves = AllCombinationsOf(allTeamMoves);

        return allCombinationMoves;
    }

    private static List<List<T>> AllCombinationsOf<T>(List<List<T>> sets)
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
    #endregion
}

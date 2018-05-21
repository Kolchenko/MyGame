using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
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

        var bestMove = MakeBestMove(startTeamPos, 3 /*must be odd*/, isBotTeam, Mathf.NegativeInfinity, Mathf.Infinity);

        UpdateTeamPosition(bestMove.Second, botTeam);

        Turn.isHumanTurn = true;
    }

    // Alpha-beta pruning
    private Pair<float, List<Node>> MakeBestMove(List<Node> teamPos, int depth, bool isBotTeam, float alpha, float beta)
    {
        Pair<float, List<Node>> bestMove = new Pair<float, List<Node>>();
        if (depth == 0)
        {
            return GetHeuristicEvaluation(teamPos);
        }
        --depth;

        List<List<Node>> allAvailableTeamMoves = new List<List<Node>>();

        if (isBotTeam)
        {
            bestMove.First = Mathf.NegativeInfinity;
            allAvailableTeamMoves = GetAllAvailableTeamMoves(botTeam); //todo: check null

            List<Node> oldTeamPosition = new List<Node>();
            foreach (var item in botTeam)
            {
                Vector2 localPos = PositionConverter.ToLocalCoordinates(new Vector3(item.tileX, 0, item.tileZ));
                oldTeamPosition.Add(BoardManager.Instance.map.graph[(int)localPos.x, (int)localPos.y]);
            }

            foreach (var teamPosition in allAvailableTeamMoves)
            {
                UpdateTeamPosition(teamPosition, botTeam);
                Pair<float, List<Node>> tmpResult = MakeBestMove(teamPosition, depth, false, alpha, beta);
                UpdateTeamPosition(oldTeamPosition, botTeam);
                if (tmpResult.First > bestMove.First)
                {
                    bestMove.First = tmpResult.First;
                    bestMove.Second = teamPosition;
                }
                alpha = Mathf.Max(alpha, bestMove.First);
                if (beta <= alpha)
                {
                    break;
                }
            }
            return bestMove;
        }
        else
        {
            bestMove.First = Mathf.Infinity;
            allAvailableTeamMoves = GetAllAvailableTeamMoves(Human.humanTeam);

            List<Node> oldTeamPosition = new List<Node>();
            foreach (var item in Human.humanTeam)
            {
                Vector2 localPos = PositionConverter.ToLocalCoordinates(new Vector3(item.tileX, 0, item.tileZ));
                oldTeamPosition.Add(BoardManager.Instance.map.graph[(int)localPos.x, (int)localPos.y]);
            }

            foreach (var teamPosition in allAvailableTeamMoves)
            {
                UpdateTeamPosition(teamPosition, Human.humanTeam);
                Pair<float, List<Node>> tmpResult = MakeBestMove(teamPosition, depth, true, alpha, beta);
                UpdateTeamPosition(oldTeamPosition, Human.humanTeam);
                if (tmpResult.First < bestMove.First)
                {
                    bestMove.First = tmpResult.First;
                    bestMove.Second = teamPosition;
                }
                beta = Mathf.Min(beta, bestMove.First);
                if (beta <= alpha)
                {
                    break;
                }
            }
            return bestMove;
        }
    }

    private void UpdateTeamPosition(List<Node> teamPosition, List<Unit> team)
    {
        for (int i = 0; i < team.Count; ++i)
        {
            float y = team[i].transform.position.y;
            Vector3 unitWorldPos = PositionConverter.ToWorldCoordinates(new Vector2(teamPosition[i].x, teamPosition[i].y));
            unitWorldPos.y = y;
            team[i].transform.position = unitWorldPos;
            team[i].UpdatePosition(team[i].transform.position.x, team[i].transform.position.z);
        }
    }

    private Pair<float, List<Node>> GetHeuristicEvaluation(List<Node> teamPosition)
    {
        int value = 0;            
        foreach (var unitPosition in teamPosition)
        {
            value += BoardManager.Instance.map.width - unitPosition.x;
        }

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
            var noDuplicateMoves = new HashSet<Node>(moves).ToList();
            allTeamMoves.Add(noDuplicateMoves);
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

﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class Bot {
    const int COUNT_OF_UNITS = 3;
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
        Node[] startTeamPos = new Node[COUNT_OF_UNITS];
        for(int i = 0; i < COUNT_OF_UNITS; ++i)
        {
            LocalPosition unitLocalPos = PositionConverter.ToLocalCoordinates(botTeam[i].worldPosition);
            Node unitStartNode = BoardManager.Instance.map.graph[unitLocalPos.x, unitLocalPos.y];
            startTeamPos[i] = unitStartNode;
        }

        var bestMove = MakeBestMove(startTeamPos, 3 /*must be odd*/, isBotTeam, Mathf.NegativeInfinity, Mathf.Infinity);

        MoveTeamPosition(bestMove.Second, botTeam);

        Turn.isHumanTurn = true;
    }

    // Alpha-beta pruning
    private Pair<float, Node[]> MakeBestMove(Node[] teamPos, int depth, bool isBotTeam, float alpha, float beta)
    {
        if (depth == 0)
        {
            return GetHeuristicEvaluation(teamPos);
        }

        Pair<float, Node[]> bestMove = new Pair<float, Node[]>();
        bestMove.First = alpha;
        List<Node[]> allAvailableTeamMoves = GetAllAvailableTeamMoves(botTeam); //todo: check null

        Node[] oldTeamPosition = new Node[COUNT_OF_UNITS];
        for (int i = 0; i < COUNT_OF_UNITS; ++i)
        {
            LocalPosition localPos = PositionConverter.ToLocalCoordinates(botTeam[i].worldPosition);
            oldTeamPosition[i] = BoardManager.Instance.map.graph[localPos.x, localPos.y];
        }

        for (int i = 0; i < allAvailableTeamMoves.Count; ++i)
        {
            Node[] teamPosition = allAvailableTeamMoves[i].ToArray();
            UpdateTeamPosition(teamPosition, botTeam);
            Pair<float, Node[]> tmpResult = MakeBestMove(teamPosition, depth - 1, !isBotTeam, -beta, -bestMove.First);
            //tmpResult.First *= -1;
            UpdateTeamPosition(oldTeamPosition, botTeam);
            if (tmpResult.First > bestMove.First)
            {
                bestMove.First = tmpResult.First;
                bestMove.Second = teamPosition;
                if (bestMove.First >= beta)
                {
                    break;
                }
            }

        }
        return bestMove;                
    }

    private void UpdateTeamPosition(Node[] teamPosition, List<Unit> team)
    {
        for (int i = 0; i < team.Count; ++i)
        {
            WorldPosition unitWorldPos = PositionConverter.ToWorldCoordinates(new LocalPosition(teamPosition[i].x, teamPosition[i].y));
            team[i].UpdatePosition(unitWorldPos);
        }
    }

    private void MoveTeamPosition(Node[] teamPosition, List<Unit> team)
    {
        for (int i = 0; i < team.Count; ++i)
        {
            float y = team[i].transform.position.y;
            WorldPosition unitWorldPos = PositionConverter.ToWorldCoordinates(new LocalPosition(teamPosition[i].x, teamPosition[i].y));
            unitWorldPos.y = y;
            team[i].transform.position = unitWorldPos.ToVector3();
            team[i].UpdatePosition(unitWorldPos);
        }
    }

    private Pair<float, Node[]> GetHeuristicEvaluation(Node[] teamPosition)
    {
        int value = 0;            
        foreach (var unitPosition in teamPosition)
        {
            value += BoardManager.Instance.map.width - unitPosition.x;
        }

        return new Pair<float, Node[]> (value, teamPosition);
    }
    
    private List<Node[]> GetAllAvailableTeamMoves(List<Unit> team)
    {
        List<Node[]> allTeamMoves = new List<Node[]>();
        foreach (var item in team)
        {
            BoardManager.selectedUnit = item;
            List<Node> moves = new List<Node>();
            LocalPosition localPos = PositionConverter.ToLocalCoordinates(item.worldPosition);
            BoardManager.Instance.GetAvailableMovementTiles(moves, BoardManager.Instance.map.graph[localPos.x, localPos.y]);
            var noDuplicateMoves = new HashSet<Node>(moves).ToArray();
            allTeamMoves.Add(noDuplicateMoves);
        }

        List<Node[]> allCombinationMoves = GetAllCombinationMoves(allTeamMoves);

        return allCombinationMoves;
    }

    List<Node[]> GetAllCombinationMoves(List<Node[]> teamMoves)
    {
        List<Node[]> allCombinations = new List<Node[]>();
        for (int i = 0; i < teamMoves[0].Length; ++i)
        {
            for (int j = 0; j < teamMoves[1].Length; ++j)
            {
                for (int k = 0; k < teamMoves[2].Length; ++k)
                {
                    Node[] poses = new Node[COUNT_OF_UNITS] { teamMoves[0][i], teamMoves[1][j], teamMoves[2][k] };
                    allCombinations.Add(poses);
                }
            }
        }

        return allCombinations;
    }

    //todo: remove this code
    #region get all moves 
    private static List<List<T>> AllCombinationsOf<T>(List<List<T>> sets)
    {
        var combinations = new List<List<T>>();

        foreach (var value in sets[0])
            combinations.Add(new List<T> { value });

        foreach (var set in sets.Skip(1))
            combinations = AddExtraSet(combinations, set);

        return combinations;
    }

    private static List<List<T>> AddExtraSet<T> (List<List<T>> combinations, List<T> set)
    {
        var newCombinations = from value in set
                              from combination in combinations
                              select new List<T>(combination) { value };

        return newCombinations.ToList();
    }
    #endregion
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class Bot
{
    const int START_INITIATIVE = 0;
    const int DEPTH_OF_TREE = 1; //todo: temporary
    int countOfUnits = 0;

    public class Pair<T, U>
    {
        public Pair()
        {
        }

        public Pair(T first, U second)
        {
            First = first;
            Second = second;
        }

        public T First { get; set; }
        public U Second { get; set; }
    };

    private List<Unit> botTeam;

    public Bot()
    {
        botTeam = BoardManager.Instance.enemyUnits;
    }

    bool isBotTeam = true;

    public void Do()
    {
        botTeam = BoardManager.Instance.enemyUnits;
        countOfUnits = botTeam.Count; // todo: remove
        Node[] startTeamPos = GetCurrentTeamPosition(botTeam);
        Node[] testTeamPos = new Node[botTeam.Count];
        scores.Clear(); //todo: remove
        var bestMove = MakeBestMove(startTeamPos, DEPTH_OF_TREE, START_INITIATIVE, isBotTeam, Mathf.NegativeInfinity, Mathf.Infinity, testTeamPos);
        botTeam = BoardManager.Instance.enemyUnits;
        MoveTeamPosition(bestMove.Second, botTeam);
        Turn.isHumanTurn = true;
    }

    List<Pair<float, Node[]>> allMoves = new List<Pair<float, Node[]>>();
    List<float> scores = new List<float>();

    // Alpha-beta pruning
    private Pair<float, Node[]> MakeBestMove(Node[] teamPos, int depth, int initiative, bool isBotTeam, float alpha, float beta, Node[] testTeamPos)
    {
        if ((DEPTH_OF_TREE == 1 && depth == 0) || (DEPTH_OF_TREE != 1 && depth == 1))
        {
            return GetHeuristicEvaluation(teamPos);
        }

        Pair<float, Node[]> bestMove = new Pair<float, Node[]>();
        bestMove.First = alpha;

        List<Unit> currentTeam = isBotTeam ? botTeam : Human.humanTeam;
        List<Node> allAvailableUnitMoves = GetAllAvailableUnitMoves(currentTeam[initiative]);

        if (initiative == currentTeam.Count - 1)
        {
            initiative = START_INITIATIVE;
            isBotTeam = !isBotTeam;
            --depth;
        }
        else
        {
            ++initiative;
        }

        for (int i = 0; i < allAvailableUnitMoves.Count; ++i)
        {
            Node unitPosition = allAvailableUnitMoves[i];

            UpdateTeamPosition(currentTeam, unitPosition, initiative == 0 ? currentTeam.Count - 1 : initiative - 1 /*prev unit*/);

            if (depth == DEPTH_OF_TREE && isBotTeam)
            {
                testTeamPos[initiative - 1] = unitPosition;             
            } else if (depth == DEPTH_OF_TREE - 1 && !isBotTeam && initiative == START_INITIATIVE)
            {
                testTeamPos[currentTeam.Count - 1] = unitPosition;
            }


            #region set newTeamPosition
            Node[] newTeamPosition = GetCurrentTeamPosition(currentTeam);
            if (initiative == START_INITIATIVE && isBotTeam && depth != 0)
            {
                newTeamPosition = GetCurrentTeamPosition(botTeam);
            }
            else if (initiative == START_INITIATIVE && !isBotTeam && depth != 0)
            {
                newTeamPosition = GetCurrentTeamPosition(Human.humanTeam);
            }
            #endregion

            Pair<float, Node[]> tmpResult = MakeBestMove(newTeamPosition, depth, initiative, isBotTeam, -beta, -bestMove.First, testTeamPos);
            UpdateTeamPosition(teamPos, currentTeam);

            if (tmpResult.First > bestMove.First)
            {
                bestMove.First = tmpResult.First;
                scores.Add(bestMove.First);
                if (((DEPTH_OF_TREE == 1 && depth == 0) || (DEPTH_OF_TREE != 1 && depth == 1)) && !isBotTeam && initiative == 0)
                {
                    bestMove.Second = new Node[testTeamPos.Length];
                    for (int j = 0; j < testTeamPos.Length; ++j)
                    {
                        bestMove.Second[j] = new Node(testTeamPos[j]);
                    }
                }
                else
                {
                    bestMove.Second = tmpResult.Second;
                }
            }

            if (-bestMove.First > beta) // todo: >
            {
                break;
            }
        }

        return bestMove;
    }

    private void UpdateTeamPosition(List<Unit> currentTeam, Node unitPosition, int initiative)
    {
        if (initiative < currentTeam.Count)
        {
            //todo: add ctor for nodes
            currentTeam[initiative].UpdatePosition(PositionConverter.ToWorldCoordinates(new LocalPosition(unitPosition.x, unitPosition.y)));
        }
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
            BoardManager.selectedUnit = team[i];
            BoardManager.Instance.GeneratePathTo(unitWorldPos.x, unitWorldPos.z);
            BoardManager.selectedUnit.MoveToEnterTile();
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

        return new Pair<float, Node[]>(value, teamPosition);
    }

    private List<Node> GetAllAvailableUnitMoves(Unit unit)
    {
        BoardManager.selectedUnit = unit;
        List<Node> moves = new List<Node>();
        LocalPosition localPos = PositionConverter.ToLocalCoordinates(unit.worldPosition);
        BoardManager.Instance.GetAvailableMovementTiles(moves, BoardManager.Instance.map.graph[localPos.x, localPos.y]);

        return moves;
    }

    private Node[] GetCurrentTeamPosition(List<Unit> team)
    {
        Node[] teamPosition = new Node[team.Count];
        for (int i = 0; i < team.Count; ++i)
        {
            LocalPosition localPos = PositionConverter.ToLocalCoordinates(team[i].worldPosition);
            teamPosition[i] = BoardManager.Instance.map.graph[localPos.x, localPos.y];
        }

        return teamPosition;
    }
}

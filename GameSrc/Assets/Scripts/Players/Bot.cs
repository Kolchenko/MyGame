using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class Bot
{
    const int START_INITIATIVE = 0;
    const int DEPTH_OF_TREE = 3; //todo: tmp

    readonly Dictionary<string, int> materialEvalue = new Dictionary<string, int>() {
        { "Spearman", 100 },
        { "Bowman",   200 },
        { "Griffin",  200 },
    };

    readonly Dictionary<string, int> positionEvalue = new Dictionary<string, int>() {
        { "Spearman", 16 },
        { "Bowman",   21 },
        { "Griffin",  26 },
    };    

    public static List<Unit> botTeam;

    public Bot()
    {
        botTeam = BoardManager.Instance.enemyUnits;
    }

    bool isBotTeam = true;

    public void Do()
    {
        if (botTeam.Count != 0)
        {
            botTeam = BoardManager.Instance.enemyUnits;
            Node[] startTeamPos = GetCurrentTeamPosition(botTeam);
            Node[] testTeamPos = new Node[botTeam.Count];
            var bestMove = MakeBestMove(startTeamPos, DEPTH_OF_TREE, START_INITIATIVE, isBotTeam, Mathf.NegativeInfinity, Mathf.Infinity, testTeamPos);    
            botTeam = BoardManager.Instance.enemyUnits;
            MoveTeamPosition(bestMove.Second, botTeam);
            Turn.isHumanTurn = true;
        }
    }

    // Alpha-beta pruning
    private Pair<float, Node[]> MakeBestMove(Node[] teamPos, int depth, int initiative, bool isBotTeam, float alpha, float beta, Node[] teamPos3lvl)
    {
        if ((DEPTH_OF_TREE == 1 && depth == 0) || (DEPTH_OF_TREE != 1 && depth == 1))
        {
            return GetHeuristicEvaluation(teamPos, isBotTeam);
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
                teamPos3lvl[initiative - 1] = unitPosition;             
            } else if (depth == DEPTH_OF_TREE - 1 && !isBotTeam && initiative == START_INITIATIVE)
            {
                teamPos3lvl[currentTeam.Count - 1] = unitPosition;
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

            Pair<float, Node[]> tmpResult = MakeBestMove(newTeamPosition, depth, initiative, isBotTeam, -beta, -bestMove.First, teamPos3lvl);
            UpdateTeamPosition(teamPos, currentTeam);

            if (tmpResult.First > bestMove.First)
            {
                bestMove.First = tmpResult.First;
                if (((DEPTH_OF_TREE == 1 && depth == 0 && !isBotTeam) || (DEPTH_OF_TREE != 1 && depth == 1 && isBotTeam)) && initiative == 0)
                {
                    bestMove.Second = new Node[teamPos3lvl.Length];
                    for (int j = 0; j < teamPos3lvl.Length; ++j)
                    {
                        bestMove.Second[j] = new Node(teamPos3lvl[j]);
                    }
                }
                else
                {
                    bestMove.Second = tmpResult.Second;
                }
            }

            if (-bestMove.First >= beta)
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

            if (BoardManager.Instance.isUnitOccupiedNodeByTeam(teamPosition[i], true))
            {
                Debug.Log(team[i].tag + ": attack to " + teamPosition[i].x + " " + teamPosition[i].y);
                Unit defUnit = BoardManager.Instance.GetUnitByNode(teamPosition[i], false);
                BoardManager.Instance.GeneratePathToEnemy(defUnit);
                var path = BoardManager.selectedUnit.currentPathToEnemy;
                if (path != null)
                {
                    unitWorldPos = PositionConverter.ToWorldCoordinates(new LocalPosition(path[path.Count - 1].x, path[path.Count - 1].y));
                }
                BoardManager.selectedUnit.Attack(defUnit);
            }
            else
            {                
                BoardManager.Instance.GeneratePathTo(unitWorldPos.x, unitWorldPos.z);
                BoardManager.selectedUnit.MoveToEnterTile();
                team[i].transform.position = unitWorldPos.ToVector3();
                team[i].UpdatePosition(unitWorldPos);
            }
        }
    }

    private Pair<float, Node[]> GetHeuristicEvaluation(Node[] teamPosition, bool isBotTeam)
    {
        int value = 0;
        foreach (var unitPosition in teamPosition)
        {
            int[,] fieldEvaluationValue = GetEvaluationField(isBotTeam);
            value += fieldEvaluationValue[unitPosition.y, unitPosition.x];
        }

        return new Pair<float, Node[]>(value, teamPosition);
    }

    private List<Node> GetAllAvailableUnitMoves(Unit unit)
    {
        BoardManager.selectedUnit = unit;
        List<Node> moves = new List<Node>();
        LocalPosition localPos = PositionConverter.ToLocalCoordinates(unit.worldPosition);
        BoardManager.Instance.GetAvailableMovementTiles(moves, BoardManager.Instance.map.graph[localPos.x, localPos.y]);

        List<Unit> availableHumanUnitsForAttack = BoardManager.Instance.GetAvailableHumanUnitsForAttack();

        foreach (var item in availableHumanUnitsForAttack)
        {
            moves.Add(BoardManager.Instance.map.graph[item.localPosition.x, item.localPosition.y]);
        }

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

    private int[,] GetEvaluationField(bool isBotTeam)
    {
        int height = BoardManager.Instance.map.height;
        int width = BoardManager.Instance.map.width;
        int[,] fieldEvaluationValue = new int[height, width];

        var team = Human.humanTeam;

        foreach (var item in team)
        {
            int startEvaluation = positionEvalue[item.tag];
            for (int i = 0; i < height; ++i)
            {
                for (int j = 0; j < width; ++j)
                {
                    int diffHeight = Math.Abs(i - item.localPosition.y);
                    int diffWidth = Math.Abs(j - item.localPosition.x);
                    int evalue = startEvaluation - Math.Max(diffHeight, diffWidth);
                    fieldEvaluationValue[i, j] += evalue;
                }
            }
        }

        foreach (var item in team)
        {
            fieldEvaluationValue[item.localPosition.y, item.localPosition.x] =  materialEvalue[item.tag];
        }

        return fieldEvaluationValue;
    }
}

using UnityEngine;
using System.Collections;
using System;

public class MoveObject : MonoBehaviour
{
    static bool inFirst = false;

    public static IEnumerator MoveUnitTo(WorldPosition moveTo, float time)
    {
        inFirst = true;
        Unit unit = BoardManager.selectedUnit;
        Vector3 startPos = unit.worldPosition.ToVector3();
        var tmpPos = PositionConverter.ToWorldCoordinates(unit.currentPath[0]).ToVector3();
        Vector3 endPos = new Vector3(tmpPos.x, moveTo.y, tmpPos.z);
        int count = 0;

        while (unit.currentPath.Count != 1)
        {
            unit.currentPath.RemoveAt(0);
            ++count;
            if (count == 1)
            {
                startPos = unit.worldPosition.ToVector3();
            }
            var tmpPos_ = PositionConverter.ToWorldCoordinates(unit.currentPath[0]).ToVector3();
            endPos = new Vector3(tmpPos_.x, moveTo.y, tmpPos_.z);

            for (float t = 0; t <= 1 * time; t += Time.deltaTime)
            {
                unit.transform.position = Vector3.Lerp(startPos, endPos, t / time);
                yield return null;
            }
            startPos = endPos;
        }
        
        unit.currentPath = null;
        inFirst = false;
        Unit.isHumanMakeTurn = true;
    }

    public static IEnumerator ApproachUnitTo(WorldPosition moveTo, float time)
    {
        inFirst = true;
        Unit unit = BoardManager.selectedUnit;
        Vector3 startPos = unit.worldPosition.ToVector3();
        var tmpPos = PositionConverter.ToWorldCoordinates(unit.currentPathToEnemy[0]).ToVector3();
        Vector3 endPos = new Vector3(tmpPos.x, moveTo.y, tmpPos.z);
        int count = 0;
        if (unit.currentPathToEnemy.Count == 1)
        {
            for (float t = 0; t <= 1 * time; t += Time.deltaTime)
            {
                unit.transform.position = Vector3.Lerp(startPos, endPos, t / time);
                yield return null;
            }
        }
        else
        {
            while (unit.currentPathToEnemy.Count != 1)
            {
                unit.currentPathToEnemy.RemoveAt(0);
                ++count;
                if (count == 1)
                {
                    startPos = unit.worldPosition.ToVector3();
                }
                var tmpPos_ = PositionConverter.ToWorldCoordinates(unit.currentPathToEnemy[0]).ToVector3();
                endPos = new Vector3(tmpPos_.x, moveTo.y, tmpPos_.z);

                for (float t = 0; t <= 1 * time; t += Time.deltaTime)
                {
                    unit.transform.position = Vector3.Lerp(startPos, endPos, t / time);
                    yield return null;
                }
                startPos = endPos;
            }
        }

        unit.currentPathToEnemy = null;
        inFirst = false;
        Unit.isHumanMakeTurn = true;
    }

    public static IEnumerator DoLast()
    {
        yield return new WaitUntil(() => inFirst == true);
        Unit.isHumanMakeTurn = false;
    }
}
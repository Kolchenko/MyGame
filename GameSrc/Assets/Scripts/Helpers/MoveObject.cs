using UnityEngine;
using System.Collections;
using System;

public class MoveObject : MonoBehaviour
{
    public static IEnumerator MoveUnit(WorldPosition moveTo)
    {
        Unit unit = BoardManager.selectedUnit;

        while (unit.transform.position != moveTo.ToVector3())
        {
            float y = unit.transform.position.y;
            while (unit.currentPath.Count != 1)
            {
                unit.currentPath.RemoveAt(0);
                WorldPosition newUnitPos = PositionConverter.ToWorldCoordinates(new LocalPosition(unit.currentPath[0].x, unit.currentPath[0].y));
                newUnitPos.y = y;
                unit.transform.position = newUnitPos.ToVector3();
                yield return new WaitForSeconds(Time.deltaTime); //todo: set time for each units
            }
        }

        unit.currentPath = null;
    }
}
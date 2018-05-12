using UnityEngine;
using System.Collections;

public class MoveObject : MonoBehaviour
{
    public static IEnumerator MoveUnit(Vector3 moveTo)
    {
        Unit unit = BoardManager.selectedUnit;

        while (unit.transform.position != moveTo)
        {
            float y = unit.transform.position.y;
            while (unit.currentPath.Count != 1)
            {
                unit.currentPath.RemoveAt(0);
                Vector3 newUnitPos = PositionConverter.ToWorldCoordinates(new Vector2(unit.currentPath[0].x, unit.currentPath[0].y));
                newUnitPos.y = y;
                unit.transform.position = newUnitPos;
                yield return new WaitForSeconds(Time.deltaTime * 3); //todo: set time for each units
            }
        }

        //unit.UpdatePosition(moveTo.x, moveTo.z);
        unit.currentPath = null;
        unit.availableMovementTiles = null;
    }
}
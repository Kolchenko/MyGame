using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackableEnemy : MonoBehaviour {
    public Texture2D attackCursor;
    public Texture2D defaultCursor;

    private void OnMouseUp()
    {
        if (CanAttack()) {
            var enemyUnit = gameObject.GetComponent<Unit>();
            var selectedUnit = BoardManager.selectedUnit;
            selectedUnit.Attack(enemyUnit);
        }
    }

    private void OnMouseEnter()
    {
        if (CanAttack())
        {
            Cursor.SetCursor(attackCursor, Vector2.zero, CursorMode.Auto);
        }
    }

    private void OnMouseExit()
    {
        Cursor.SetCursor(defaultCursor, Vector2.zero, CursorMode.Auto);
    }

    private bool CanAttack()
    {
        var enemyUnit = gameObject.GetComponent<Unit>();
        var selectedUnit = BoardManager.selectedUnit;
        BoardManager.Instance.GeneratePathToEnemy(enemyUnit);
        if (selectedUnit.currentPathToEnemy != null && selectedUnit.currentPathToEnemy.Count <= selectedUnit.distance + 1)
        {
            return true;
        }

        return false;
    }
}

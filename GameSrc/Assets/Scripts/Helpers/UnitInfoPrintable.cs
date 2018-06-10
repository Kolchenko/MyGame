using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitInfoPrintable : MonoBehaviour {
    private void Update () {
        if (Input.GetMouseButtonDown((int)MouseButton.RIGHT))
        {
            OnRightClick();
        }
    }

    private void OnRightClick()
    {
        Ray clickPoint = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitPoint;
        
        if (Physics.Raycast(clickPoint, out hitPoint))
        {
            for (int i = 0; i < 3 /*count of player's team*/; i++)
            {
                if (hitPoint.collider == BoardManager.Instance.playerUnits[i].GetComponent<Collider>())
                {
                    printInfo(BoardManager.Instance.playerUnits[i]);
                }
                else if (hitPoint.collider == BoardManager.Instance.enemyUnits[i].GetComponent<Collider>())
                {
                    printInfo(BoardManager.Instance.enemyUnits[i]);
                }
            }
        }
    }

    private void printInfo(Unit unit)
    {
        Debug.Log("Distance: " + unit.distance);
        Debug.Log("Damage: " + unit.damage);
        Debug.Log("Defense: " + unit.defense);
    }
}

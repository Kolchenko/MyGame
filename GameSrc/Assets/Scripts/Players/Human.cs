using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

public class Human {

    public static List<Unit> humanTeam;

    public Human()
    {
        humanTeam = BoardManager.Instance.playerUnits.OfType<Unit>().ToList();
    }

    public static bool isDoUnit = false;

    public static IEnumerator Do()
    {
        foreach (var item in BoardManager.Instance.playerUnits)
        {
            BoardManager.Instance.SelectUnit(item);
            yield return new WaitUntil(() => Unit.isHumanMakeTurn == true && isDoUnit == true);
            Unit.isHumanMakeTurn = false;
            isDoUnit = false;
            BoardManager.Instance.DeselectUnit();
        }

        Turn.isBotTurn = true;
    }
}

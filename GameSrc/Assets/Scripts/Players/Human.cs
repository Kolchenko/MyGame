﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

public class Human {

    public static List<Unit> units;

    public Human()
    {
        units = BoardManager.Instance.playerUnits.OfType<Unit>().ToList();
    }

    public static IEnumerator Do()
    {
        units.OrderBy(dist => dist.distance);

        foreach (var item in units)
        {
            item.SelectUnit();
            // todo: остальные юниты задизейблить совсем, мышь доступна только на выделенных тайлах
            yield return new WaitUntil(() => Unit.isHumanMakeTurn == true);
            Unit.isHumanMakeTurn = false;
            BoardManager.Instance.DeselectUnit();
        }

        Turn.isBotTurn = true;
    }
}
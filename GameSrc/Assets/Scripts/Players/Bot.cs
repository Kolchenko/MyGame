using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Bot {
    private List<Unit> units;

    public Bot()
    {
        units = BoardManager.Instance.enemyUnits.OfType<Unit>().ToList();
    }

    public void Do()
    {
        System.Random rand = new System.Random();
        units.OrderBy(x => x.distance);

        foreach (var item in units)
        {
            int row = rand.Next(14);
            int col = rand.Next(10);
            item.transform.position = PositionConverter.ToWorldCoordinates(new Vector2(row, col));
            item.UpdatePosition(item.transform.position.x, item.transform.position.z);
        }
        Turn.isHumanTurn = true;
    }
}

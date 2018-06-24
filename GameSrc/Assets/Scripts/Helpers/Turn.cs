using System.Collections;
using UnityEngine;

class Turn
{
    public static bool isBotTurn = false;
    public static bool isHumanTurn = true;

    public static void RestartFlags()
    {
        isBotTurn = false;
        isHumanTurn = true;
    }
}

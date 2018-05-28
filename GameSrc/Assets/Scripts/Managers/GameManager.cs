using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum MouseButton
{
    LEFT = 0,
    RIGHT,
    MIDDLE
}

public class GameManager : MonoBehaviour {
    public static GameManager Instance { get; private set; }
    private Bot bot;
    private Human human;

    private void Awake()
    {
        Debug.Log("GameManagerAwake");
        Instance = this;
        bot = new Bot();
        human = new Human();
    }

    private void Update()
    {
        if (BoardManager.Instance != null)
        {
            while (Turn.isHumanTurn)
            {
                StartCoroutine(Human.Do());
                Turn.isHumanTurn = false;
            }

            while (Turn.isBotTurn)
            {
                bot.Do();
                Turn.isBotTurn = false;                
            }
        }
    }  
}

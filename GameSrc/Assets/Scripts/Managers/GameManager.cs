using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
    public event EventHandler gameOverEvent;
    public static bool isGameOver = false;
    public static bool isMapSetUp = false;
    public static bool isReloadGame = false;

    private void OnGameOver()
    {
        if (gameOverEvent != null)
        {
            gameOverEvent(this, EventArgs.Empty);
            isGameOver = true;
        }
    }

    private void Awake()
    {
        Debug.Log("GameManagerAwake");
        Instance = this;
        isReloadGame = false;
        bot = new Bot();
        human = new Human();
        Turn.RestartFlags();
    }

    private void Update()
    {
        if (BoardManager.Instance != null && !isGameOver)
        {
            while (Turn.isHumanTurn)
            {
                StartCoroutine(Human.Do());
                checkGameState();
                Turn.isHumanTurn = false;
            }

            while (Turn.isBotTurn)
            {
                bot.Do();
                checkGameState();
                Turn.isBotTurn = false;
            }
        }
    } 

    public void checkGameState()
    {
        if (BoardManager.Instance.playerUnits.Count == 0 || BoardManager.Instance.enemyUnits.Count == 0)
        {
            OnGameOver();
        }        
    }

    public bool isWon()
    {
        return BoardManager.Instance.enemyUnits.Count == 0;
    }
}

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour {

    public RectTransform gameOverPanel;
    public TextMeshProUGUI textMeshPro;
    
	void Start () {
        GameManager.Instance.gameOverEvent += OnGameOverEvent;
	}

    private void OnGameOverEvent(object sender, System.EventArgs e)
    {
        gameOverPanel.gameObject.SetActive(true);
        textMeshPro.text = GameManager.Instance.isWon() ? "ПОБЕДА" : "ПОРАЖЕНИЕ";
    }

}

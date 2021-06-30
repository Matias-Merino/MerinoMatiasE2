using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UI;

public enum GameState
{
    Wait,
    GamePlay,
    GameOver,
    RoundOver
}
public class GameManager : MonoBehaviour
{
    float timer = 0f;
    float gameOverTimer = 150f;
    public GameObject timerText;

    public static int enemyAmount = 0;
    public static int enemyKilled = 0;
    public GameState gameState;

    private static GameManager _instance;
    public static GameManager Instance
    {
        get
        {
            return _instance;
        }
    }

    private void Awake()
    {
        _instance = this;
    }

    public delegate void FNotify();
    public static event FNotify OnWait;
    public static event FNotify OnGameplay;
    public static event FNotify OnRoundOver;
    public static event FNotify OnGameOver;

    private void OnDisable()
    {
        PlayerController.OnPlayerDeath -= PlayerController_OnPlayerDeath;
        Enemy.OnEnemyDeath -= Enemy_OnEnemyDeath;
    }

    private void OnEnable()
    {
        PlayerController.OnPlayerDeath += PlayerController_OnPlayerDeath;
        Enemy.OnEnemyDeath += Enemy_OnEnemyDeath;
    }

    private void PlayerController_OnPlayerDeath()
    {
        OnGameOver.Invoke();
    }
    private void Enemy_OnEnemyDeath()
    {
        enemyKilled++;
    }

    private void UpdateGS(GameState currentState)
    {
        gameState = currentState;

        switch (currentState) 
        {
            case GameState.Wait:
                HandleWait();
                break;
            case GameState.GamePlay:
                HandleGamePlay();
                break;
            case GameState.GameOver:
                HandleGameOver();
                break;
            case GameState.RoundOver:
                HandleRoundOver();
                break;
        }
    }

    private void HandleRoundOver()
    {
        
    }

    private void HandleGameOver()
    {
        
    }

    private void HandleGamePlay()
    {
        
    }

    private void HandleWait()
    {
        
    }

    private void Start()
    {
        gameState = GameState.Wait;
        OnWait.Invoke();
        timerText.GetComponent<Text>().text = gameOverTimer.ToString("0");
        Debug.Log(enemyAmount);
    }

    private void Update()
    {
        if (gameState == GameState.Wait)
        {
            timer += Time.deltaTime;
        }
        if (timer >= 2f)
        {
            Debug.Log("Game Start");
            gameState = GameState.GamePlay;
            OnGameplay.Invoke();
            timer=0;
        }

        if (gameState == GameState.GamePlay)
        {
            gameOverTimer -= Time.deltaTime;
            timerText.GetComponent<Text>().text = gameOverTimer.ToString("0");
            if (gameOverTimer<=0)
            {
                gameState = GameState.GameOver;
                OnGameOver.Invoke();
                Debug.Log("Game Over");
            }
        }

        if (gameState == GameState.GamePlay)
        {
            if (enemyKilled >= enemyAmount)
            {
                gameState = GameState.RoundOver;
                OnRoundOver.Invoke();
                Debug.Log("You Win!!");
            }
        }
    }

}
